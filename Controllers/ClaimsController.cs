﻿using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MvcCode.Controllers
{
    public class AutenticacionDigital
    {
        public string sub { get; set; }
        public string auth_time { get; set; }
        public string idp { get; set; }
        public string acr { get; set; }
        public string name { get; set; }
        public string s_hash { get; set; }
        public string Identificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string LOA { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string nickname { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string DireccionJSON { get; set; }
        public string preferred_username { get; set; }
        public string email { get; set; }
        public string email_verified { get; set; }
        public string amr { get; set; }
    }
    public class ClaimsController : Controller
    {
        private readonly IConfiguration _configuration;
        public ClaimsController(IConfiguration _config)
        {
            _configuration = _config;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var httpClient = new HttpClient();
            var userInfo = new UserInfoRequest();

            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
            //You get the user's first and last name below:
            ViewBag.Name = userClaims?.FindFirst("audd")?.Value;

            // The 'preferred_username' claim can be used for showing the username
            ViewBag.Username = userClaims?.FindFirst("audd")?.Value;

            // The subject/ NameIdentifier claim can be used to uniquely identify the user across the web
            ViewBag.Subject = userClaims?.FindFirst("sub")?.Value;

            // TenantId is the unique Tenant Id - which represents an organization in Azure AD
            ViewBag.TenantId = userClaims?.FindFirst("isss")?.Value;
            string authority = _configuration.GetValue<string>(
               "ServerSettings:authority");
            userInfo.Address = authority + "/connect/userinfo";
            userInfo.Token = userClaims?.FindFirst("access_token")?.Value;

            var userInfoProfile = await httpClient.GetUserInfoAsync(userInfo);

            ViewBag.userClaims = userInfoProfile.Claims;

            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> GetClaims() {
            if (!User.Identity!.IsAuthenticated)
                return Json(new object());
            var httpClient = new HttpClient();
            var userInfo = new UserInfoRequest();

            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
            //You get the user's first and last name below:
            ViewBag.Name = userClaims?.FindFirst("audd")?.Value;

            // The 'preferred_username' claim can be used for showing the username
            ViewBag.Username = userClaims?.FindFirst("audd")?.Value;

            // The subject/ NameIdentifier claim can be used to uniquely identify the user across the web
            ViewBag.Subject = userClaims?.FindFirst("sub")?.Value;

            // TenantId is the unique Tenant Id - which represents an organization in Azure AD
            ViewBag.TenantId = userClaims?.FindFirst("isss")?.Value;
            string authority = _configuration.GetValue<string>(
               "ServerSettings:authority");
            userInfo.Address = authority + "/connect/userinfo";
            userInfo.Token = userClaims?.FindFirst("access_token")?.Value;

            var userInfoProfile = await httpClient.GetUserInfoAsync(userInfo);
            AutenticacionDigital result = new AutenticacionDigital();
            foreach (var claim in userInfoProfile.Claims)
            {
                Type t = result.GetType();
                var p = t.GetProperty(claim.Type);
                if (p != null)
                {
                    p.SetValue(result, claim.Value);
                }
            }

            return Json(result);
        }
    }
}