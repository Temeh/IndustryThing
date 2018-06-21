using Flurl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public enum CharacterEnum
    {
        BuildCorp,
        EmpireDonkey
    }

    public class Login
    {
        CharacterEnum typeenum;

        HttpServer httpServer;

        bool authed = false;
        AuthToken token;
        string refreshToken;
        Thread blockThread, retryThread;
        AutoResetEvent gotAuth, finishAuth;

        public Login(CharacterEnum e)
        {
            typeenum = e;

            switch (typeenum)
            {
                case CharacterEnum.BuildCorp:
                    refreshToken = db.Settings.BuildCorpRefreshToken;
                    break;
                case CharacterEnum.EmpireDonkey:
                    refreshToken = db.Settings.EmpireDonkeyRefreshToken;
                    break;
            }

            StaticInfo.AuthCompleted += StaticInfo_AuthCompleted;

            go();
        }

        void go()
        {
            if (string.IsNullOrEmpty(db.Settings.ESIClientId) || string.IsNullOrEmpty(db.Settings.ESISecret))
                setup();
            else if (string.IsNullOrEmpty(refreshToken))
                auth();
            else
                refresh();
        }

        void refresh()
        {
            try
            {
                TokenHandler.FromRefreshToken(refreshToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("....Error using refresh token for " + typeenum.ToString() + ", trying auth instead");
                auth(true);
            }
        }

        void auth(bool error = false)
        {
            if (error)
            {
                Console.WriteLine("....Press ENTER to open auth window for: " + typeenum.ToString());
                Console.WriteLine("....Or type 'setup' and press ENTER to (re)configure ESI client/secret");

                var input = Console.ReadLine();
                if (input != null && input.ToLower() == "setup")
                {
                    setup();
                    return;
                }
            }
            else
            {
                Console.WriteLine("....Press any key to open auth window for: " + typeenum.ToString());

                Console.ReadKey();
            }

            using (httpServer = new HttpServer(db.Settings.GetScopes(typeenum)))
            {
                System.Threading.Tasks.Task.Run(() => httpServer.Start(new CancellationToken()));
                gotAuth = new AutoResetEvent(false);
                finishAuth = new AutoResetEvent(false);
                blockThread = new Thread(blockThreadMethod);
                blockThread.IsBackground = true;
                blockThread.Start();
                retryThread = new Thread(retryThreadMethod);
                retryThread.IsBackground = true;
                retryThread.Start();

                var url = db.Settings.URL.AppendPathSegment("auth");
                try
                {
                    Process.Start(url);
                    Console.WriteLine("....Please log in via the browser window");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("....Failed to open browser window for authentication.");
                    Console.WriteLine("....Please navigate to " + url + " manually");
                }

                // Wait for background thread
                finishAuth.WaitOne();
            }

            if (authed)
                saveRefreshToken();
            else
            {
                Console.WriteLine("....Failed to authenticate for: " + typeenum.ToString());
                Console.WriteLine("....Press X to exit or any key to retry");

                if (Console.ReadKey().KeyChar.ToString().ToUpper() != "X")
                {
                    auth(true);
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to terminate program");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        void setup()
        {
            var clientId = getClientId();
            var secret = getSecret();

            db.Settings.ESIClientId = clientId;
            db.Settings.ESISecret = secret;
            // We got new login, clear the old settings
            db.Settings.BuildCorpAccessToken = null;
            db.Settings.BuildCorpRefreshToken = null;
            db.Settings.EmpireDonkeyAccessToken = null;
            db.Settings.EmpireDonkeyRefreshToken = null;

            auth();
        }

        string getClientId()
        {
            string clientId = null;
            do
            {
                Console.WriteLine("....Please input ESI Client ID and press ENTER");
                clientId = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(clientId));

            return clientId.Trim();
        }

        string getSecret()
        {
            string secret = null;
            do
            {
                Console.WriteLine("....Please input ESI Secret Key and press ENTER");
                secret = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(secret));

            return secret.Trim();
        }

        void blockThreadMethod()
        {
            // Wait for the callback
            gotAuth.WaitOne();
            // Callback received, allow main thread to continue
            finishAuth.Set();
            retryThread.Abort();
        }

        void retryThreadMethod()
        {
            int seconds = 0;
            while (!authed)
            {
                seconds += 1;
                // Wait 10 seconds for callback
                Thread.Sleep(1000);
                //while (!gotAuth.WaitOne(10000))
                //{
                if (seconds % 10 == 0)
                    Console.WriteLine("....Didn't receive callback within " + seconds + " seconds. Press C to cancel");

                if (Console.KeyAvailable && Console.ReadKey().KeyChar.ToString().ToUpper() == "C")
                {
                    Console.WriteLine();
                    // No more waiting
                    finishAuth.Set();
                    break;
                }
                //}
            }
        }

        private void StaticInfo_AuthCompleted(AuthToken token)
        {
            StaticInfo.AuthCompleted -= StaticInfo_AuthCompleted;
            authed = true;
            this.token = token;
            Console.WriteLine("....Authentication of " + typeenum.ToString() + " (" + token.CharacterName + ") successful");
            SetTokenStuff();
            // Tell background thread we got callback
            gotAuth?.Set();
        }

        void saveRefreshToken()
        {
            if (token != null && !string.IsNullOrEmpty(token.RefreshToken))
            {
                switch (typeenum)
                {
                    case CharacterEnum.BuildCorp:
                        db.Settings.BuildCorpRefreshToken = token.RefreshToken;
                        break;
                    case CharacterEnum.EmpireDonkey:
                        db.Settings.EmpireDonkeyRefreshToken = token.RefreshToken;
                        break;
                }
                db.Settings.SaveSettings();
            }
        }

        void SetTokenStuff()
        {
            if (token != null && !string.IsNullOrEmpty(token.AccessToken))
            {
                switch (typeenum)
                {
                    case CharacterEnum.BuildCorp:
                        db.Settings.BuildCorpCharacterId = token.CharacterID;
                        break;
                    case CharacterEnum.EmpireDonkey:
                        db.Settings.EmpireDonkeyCharacterId = token.CharacterID;
                        break;
                }

                var characterResponse = StaticInfo.GetESIResponse<Character>("/characters/{character_id}/", typeenum);

                switch (typeenum)
                {
                    case CharacterEnum.BuildCorp:
                        db.Settings.BuildCorpAccessToken = token.AccessToken;
                        db.Settings.BuildCorpCorporationId = characterResponse.Result.corporation_id;
                        break;
                    case CharacterEnum.EmpireDonkey:
                        db.Settings.EmpireDonkeyAccessToken = token.AccessToken;
                        db.Settings.EmpireDonkeyCorporationId = characterResponse.Result.corporation_id;
                        break;
                }
            }
        }
    }
}
