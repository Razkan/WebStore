﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebStore.Db;
using WebStore.Model.Accounts;
using WebStore.Model.Session;

namespace WebStore.API
{
    public class SessionController : JsonApiController
    {
        [Route("api/session/{token}")]
        public async Task<object> Get(string token)
        {
            var session = await Database.SelectAsync<Session>($"{nameof(Session.Token)}='{token}'");
            if (session == null) return new {Expired = true};
            if (session.Expired()) return new {Expired = true};
            return new {Expires = session.Expires()};
        }

        protected override async Task<object> HttpGet()
        {
            var headers = Request.Headers;
            if (!headers.TryGetValues("username", out var usernames))
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            if (!headers.TryGetValues("password", out var passwords))
                throw new HttpResponseException(HttpStatusCode.Forbidden);

            var username = usernames.FirstOrDefault();
            var password = passwords.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(username)) throw new HttpResponseException(HttpStatusCode.Forbidden);
            if (string.IsNullOrWhiteSpace(password)) throw new HttpResponseException(HttpStatusCode.Forbidden);

            var account = await Database.SelectAsync<Account>($"{nameof(Account.Username)}='{username}'",
                $"{nameof(Account.Password)}='{password}'");

            if (account == null) throw new HttpResponseException(HttpStatusCode.Forbidden);

            var session = await Database.SelectAsync<Session>($"{nameof(Session.Account)}='{account.Id}'");

            if (session == null)
            {
                session = Session.Make(account, TimeSpan.FromMinutes(30));
                await Database.InsertAsync(session);
            }
            else
            {
                session.Refresh();
                await Database.UpdateAsync(session);
            }

            return new
            {
                session.Token,
                session.TTL,
                Expires = session.Expires()
            };
        }
    }

    //public class AuthenticateForm
    //{
    //    public string Test { get; set; }
    //}
}