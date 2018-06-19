using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IndustryThing.ESI
{
    public static class Strings
    {
        public const string Grant_Type = "grant_type";
        public const string Authorization_Code = "authorization_code";
        public const string Refresh_Token = "refresh_token";
    }

    public static class TokenHandler
    {
        public static void FromAuthorizationCode(string code)
        {
            var body = new Dictionary<string, string>
            {
                { Strings.Grant_Type, Strings.Authorization_Code },
                { "code", code }
            };

            GetToken(code, body);
        }

        public static void FromRefreshToken(string refreshToken)
        {
            var body = new Dictionary<string, string>
            {
                { Strings.Grant_Type, Strings.Refresh_Token },
                { Strings.Refresh_Token, refreshToken }
            };

            GetToken(refreshToken, body);
        }

        private static void GetToken(string code, Dictionary<string, string> body)
        {
            var content = new FormUrlEncodedContent(body);
            var result = db.Settings.AuthURL
                .AppendPathSegment("token")
                .WithBasicAuth(db.Settings.ESIClientId, db.Settings.ESISecret)
                .PostAsync(content)
                .ReceiveString()
                .Result;

            var obj = JObject.Parse(result);
            var token = new AuthToken()
            {
                AccessToken = obj.SelectToken("access_token").Value<string>(),
                Expires = obj.SelectToken("expires_in").Value<int>(),
                RefreshToken = obj.SelectToken(Strings.Refresh_Token).Value<string>()
            };

            if (body[Strings.Grant_Type] == Strings.Authorization_Code)
                token.AuthorizationToken = code;

            GetCharacterDetails(ref token);

            StaticInfo.Completed(token);
        }

        private static void GetCharacterDetails(ref AuthToken token)
        {
            var character = db.Settings.AuthURL
                .AppendPathSegment("verify")
                .WithOAuthBearerToken(token.AccessToken)
                .GetAsync()
                .ReceiveJson()
                .Result;

            token.CharacterName = character.CharacterName;
            token.CharacterID = (int)character.CharacterID;
            token.CharacterOwnerHash = character.CharacterOwnerHash;
            token.Scopes = character.Scopes;
        }
    }
}
