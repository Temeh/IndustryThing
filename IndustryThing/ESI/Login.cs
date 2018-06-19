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
    public enum typeenum
    {
        BuildCorp,
        EmpireDonkey
    }

    public class Login
    {
        typeenum typeenum;

        HttpServer httpServer;

        bool authed = false;
        AuthToken token;
        string refreshToken;

        public Login(typeenum e)
        {
            typeenum = e;

            switch(typeenum)
            {
                case typeenum.BuildCorp:
                    refreshToken = db.Settings.BuildCorpRefreshToken;
                    break;
                case typeenum.EmpireDonkey:
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
                Console.WriteLine("Error using refresh token, trying auth instead");
                auth();
            }
        }

        void auth()
        {
            Console.WriteLine("SSO Authentication for: " + typeenum.ToString());
            Console.WriteLine("Press any key to open auth window");

            Console.ReadKey();

            using (httpServer = new HttpServer())
            {
                System.Threading.Tasks.Task.Run(() => httpServer.Start(new CancellationToken()));

                var url = db.Settings.URL.AppendPathSegment("auth");
                try
                {
                    Process.Start(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to open browser window for authentication.");
                    Console.WriteLine("Please navigate to " + url + " manually");
                }

                loop();
            }

            saveRefreshToken();
        }

        private void StaticInfo_AuthCompleted(AuthToken token)
        {
            StaticInfo.AuthCompleted -= StaticInfo_AuthCompleted;
            authed = true;
            this.token = token;
            Console.WriteLine("Authentication of " + typeenum.ToString() + " successful");
            setAccessToken();
        }

        void loop()
        {
            Console.WriteLine("Press any key when auth is successful");
            Console.ReadKey();

            if (!authed)
            {
                Console.WriteLine("Authentication callback not yet registered. Keep waiting? Y/N");

                var k = Console.ReadLine();

                if (k == "N")
                {
                    Console.WriteLine("Application terminating!");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                else
                    loop();
            }
        }

        void saveRefreshToken()
        {
            if (token != null && !string.IsNullOrEmpty(token.RefreshToken))
            {
                switch (typeenum)
                {
                    case typeenum.BuildCorp:
                        db.Settings.BuildCorpRefreshToken = token.RefreshToken;
                        db.Settings.BuildCorpAccessToken = token.AccessToken;
                        break;
                    case typeenum.EmpireDonkey:
                        db.Settings.EmpireDonkeyRefreshToken = token.RefreshToken;
                        db.Settings.EmpireDonkeyAccessToken = token.AccessToken;
                        break;
                }
                db.Settings.SaveRefreshTokens();
            }
        }

        void setAccessToken()
        {
            if (token != null && !string.IsNullOrEmpty(token.AccessToken))
            {
                switch (typeenum)
                {
                    case typeenum.BuildCorp:
                        db.Settings.BuildCorpAccessToken = token.AccessToken;
                        break;
                    case typeenum.EmpireDonkey:
                        db.Settings.EmpireDonkeyAccessToken = token.AccessToken;
                        break;
                }
            }
        }
    }
}
