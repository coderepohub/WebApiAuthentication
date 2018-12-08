using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Xml;
using AuthWebAPI.Models;
using Microsoft.Azure.Documents.Client;
using System.Threading.Tasks;
using System.ServiceModel.Discovery;

namespace AuthWebAPI.App_Start
{
    public class APIAuthorization : AuthorizeAttribute
    {
      
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            bool isAuthorized = false;
            try
            {
                var authorization = actionContext.Request.Headers.Authorization.ToString();
                string token = authorization.Contains("Bearer") ? authorization.Substring(7) : authorization;

                string metaDataAddress = ConfigurationManager.AppSettings["MetadataAddress"];
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParams = new TokenValidationParameters()
                {
                    ValidAudience = ConfigurationManager.AppSettings["Audiance"],
                    ValidIssuer = ConfigurationManager.AppSettings["Issuer"],
                    IssuerSigningKeys = GetIssuerSigninCertificates(metaDataAddress)
                };

              //  Microsoft.IdentityModel.Tokens.SecurityToken validatedToken;
                var jsonToken = tokenHandler.ReadToken(token);

                var payload = ((System.IdentityModel.Tokens.Jwt.JwtSecurityToken)jsonToken).Payload;
                var claims = payload.Claims;
                var role = claims?.Where(t => t.Type == "roles")?.Select(t=>t.Value)?.FirstOrDefault();

                var aud = claims?.Where(t => t.Type == "aud")?.Select(t => t.Value)?.FirstOrDefault();

                var iss = claims?.Where(t => t.Type == "iss")?.Select(t => t.Value)?.FirstOrDefault();


                if (aud!=ConfigurationManager.AppSettings["Audiance"]|| iss != ConfigurationManager.AppSettings["Issuer"]|| DateTime.UtcNow > jsonToken.ValidTo)
                {
                    isAuthorized = false;
                    return isAuthorized;

                }


              //  Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParams, out validatedToken);

                //ClaimsIdentity claimsId = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
                //var appRoles = new List<string>();
                //foreach (Claim claim in ClaimsPrincipal.Current.FindAll(claimsId.RoleClaimType))
                //{
                //    appRoles.Add(claim.Value.ToUpper());
                //}

                if (!string.IsNullOrEmpty(role))
                {
                    isAuthorized = true;
                    actionContext.Request.Headers.Add("customRoles", role);
                }
            }
            catch (Exception ex)
            {
                isAuthorized = false;


            }
            return isAuthorized;
        }

        private static IEnumerable<X509SecurityKey> GetIssuerSigninCertificates(string metaDataAddress)
        {
            List<X509SecurityKey> tokens = new List<X509SecurityKey>();
            if (metaDataAddress == null)
            {
                throw new ArgumentNullException(metaDataAddress);
            }
            using (XmlReader metadatReader = XmlReader.Create(metaDataAddress))
            {
                MetadataSerializer serializer = new MetadataSerializer()
                {
                    //No need to disable
                    CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None
                };

                EntityDescriptor metaData = serializer.ReadMetadata(metadatReader) as EntityDescriptor;
                if (metaData != null)
                {
                    SecurityTokenServiceDescriptor securityTokenServiceDesc = metaData.RoleDescriptors.OfType<SecurityTokenServiceDescriptor>().First();
                    if (securityTokenServiceDesc != null)
                    {
                        IEnumerable<X509RawDataKeyIdentifierClause> x509DataClauses = securityTokenServiceDesc.Keys.Where(key => key.KeyInfo != null
                                            && (key.Use == KeyType.Signing || key.Use == KeyType.Unspecified)).
                                            Select(key => key.KeyInfo.OfType<X509RawDataKeyIdentifierClause>().First());
                        tokens.AddRange(x509DataClauses.Select(token => new X509SecurityKey(new System.Security.Cryptography.X509Certificates.X509Certificate2(token.GetX509RawData()))));

                    }
                    else
                    {
                        throw new InvalidOperationException("There is no role Descriptor of Type SecurityTokenServiceSpecifier in the metadata");
                    }
                }
                else
                {
                    throw new Exception("Invalid federation metadata document");
                }

            }
            return tokens;
        }


        private static void ValidateJWTToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
        }

        /*
        private async Task<ClaimsPrincipal> ValidateIdentityToken(string idToken)
        {
            var user = await ValidateJwt(idToken);

            var nonce = user.FindFirst("nonce")?.Value ?? "";
            if (!string.Equals(nonce, "random_nonce")) throw new Exception("invalid nonce");

            return user;
        }

        
        private static async Task<ClaimsPrincipal> ValidateJwt(string jwt)
        {
            
            // read discovery document to find issuer and key material
            var disco = await DiscoveryClient.GetAsync(Constants.Authority);

            var keys = new List<System.IdentityModel.Tokens.SecurityKey>();
            foreach (var webKey in disco.KeySet.Keys)
            {
                var e = Base64Url.Decode(webKey.E);
                var n = Base64Url.Decode(webKey.N);

                var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
                {
                    KeyId = webKey.Kid
                };

                keys.Add(key);
            }

            var parameters = new TokenValidationParameters
            {
                ValidIssuer = disco.Issuer,
                ValidAudience = "mvc.manual",
                IssuerSigningKeys = keys,

                NameClaimType = JwtClaimTypes.Name,
                RoleClaimType = JwtClaimTypes.Role
            };

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            var user = handler.ValidateToken(jwt, parameters, out var _);
            return user;
        }
        */

    }
}