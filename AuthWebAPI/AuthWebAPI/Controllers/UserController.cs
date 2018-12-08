using AuthWebAPI.Models;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace AuthWebAPI.Controllers
{
    public class UserController : ApiController
    {
        static DocumentClient client;
        static readonly string endpointUrl = ConfigurationManager.AppSettings["DocDbEndPointUrl"].ToString();
        static readonly string authorizationKey = ConfigurationManager.AppSettings["DocDbAuthorizationKey"].ToString();
        static readonly string databaseName = ConfigurationManager.AppSettings["databaseName"].ToString();
        static readonly string collectionName = ConfigurationManager.AppSettings["collectionName"].ToString();

        [ResponseType(typeof(UserAttribute))]
        [HttpGet]
        public IHttpActionResult GetUserAttribute(string guId)
        {
            var role = this.Request.Headers.GetValues("customRoles")?.FirstOrDefault();
            UserAttribute userAttr = GetUserAttr(role);
            if (userAttr==null)
            {
                return BadRequest();
            }
            userAttr.guId = guId;
            userAttr.role = null;
            return Ok(userAttr);
        }

        /// <summary>
        /// Get UserAttribute on Role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private UserAttribute GetUserAttr(string role)
        {

            return ExecuteSimpleQuery(databaseName, collectionName, role);
        }

        // ADD THIS PART TO YOUR CODE
        private static UserAttribute ExecuteSimpleQuery(string databaseName, string collectionName, string role)
        {
            using (client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
            {

                // Set some common query options
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

                string query = String.Format("SELECT * FROM C WHERE C.role={0}", role);
                // Now execute the same query via direct SQL
                //IQueryable<UserAttribute> userAttr = client.CreateDocumentQuery<UserAttribute>(
                //        UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                //        query,
                //        queryOptions);

                var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);

                UserAttribute userAttr = client.CreateDocumentQuery<UserAttribute>(collectionLink)
                                          .Where(so => so.role == role)
                                          .AsEnumerable()
                                          .FirstOrDefault();



                return userAttr;

            }
        }
    }
}
