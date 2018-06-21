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
        Thread inputThread;
        AutoResetEvent getAuth, gotAuth;

        public Login(CharacterEnum e)
        {
            typeenum = e;

            switch(typeenum)
            {
                case CharacterEnum.BuildCorp:
                    refreshToken = db.Settings.BuildCorpRefreshToken;
                    break;
                case CharacterEnum.EmpireDonkey:
                    refreshToken = db.Settings.EmpireDonkeyRefreshToken;
                    break;
            }

            StaticInfo.AuthCompleted += StaticInfo_AuthCompleted;

            if (string.IsNullOrEmpty(refreshToken))
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
                auth();
            }
        }

        void auth()
        {
            Console.WriteLine("....Press any key to open auth window for: " + typeenum.ToString());

            Console.ReadKey();

            using (httpServer = new HttpServer(db.Settings.GetScopes(typeenum)))
            {
                System.Threading.Tasks.Task.Run(() => httpServer.Start(new CancellationToken()));
                getAuth = new AutoResetEvent(false);
                gotAuth = new AutoResetEvent(false);
                inputThread = new Thread(backgroundThread);
                inputThread.IsBackground = true;
                inputThread.Start();

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
                gotAuth.WaitOne();
            }

            saveRefreshToken();
        }

        void backgroundThread()
        {
            // Wait for the callback
            getAuth.WaitOne();
            // Callback received, allow main thread to continue
            gotAuth.Set();
        }

        private void StaticInfo_AuthCompleted(AuthToken token)
        {
            StaticInfo.AuthCompleted -= StaticInfo_AuthCompleted;
            authed = true;
            this.token = token;
            Console.WriteLine("....Authentication of " + typeenum.ToString() + " successful");
            SetTokenStuff();
            // Tell background thread we got callbac
            getAuth?.Set();
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
                db.Settings.SaveRefreshTokens();
            }
        }

        void SetTokenStuff()
        {
            if (token != null && !string.IsNullOrEmpty(token.AccessToken))
            {
                var characterResponse = StaticInfo.GetESIResponse<Character>("characters/" + token.CharacterID + "/", typeenum);

                switch (typeenum)
                {
                    case CharacterEnum.BuildCorp:
                        db.Settings.BuildCorpAccessToken = token.AccessToken;
                        db.Settings.BuildCorpCharacterId = token.CharacterID;
                        db.Settings.BuildCorpCorporationId = characterResponse.Result.corporation_id;
                        break;
                    case CharacterEnum.EmpireDonkey:
                        db.Settings.EmpireDonkeyAccessToken = token.AccessToken;
                        db.Settings.EmpireDonkeyCharacterId = token.CharacterID;
                        db.Settings.EmpireDonkeyCorporationId = characterResponse.Result.corporation_id;
                        break;
                }
            }
        }
    }
}
