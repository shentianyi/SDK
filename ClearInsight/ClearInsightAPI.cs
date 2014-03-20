﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearInsight.Model;
using ClearInsight.Exception;
using ClearInsight.Validation;
using RestSharp;
using RestSharp.Serializers;
namespace ClearInsight
{
    /// <summary>
    /// Class <c>ClierInsightAPI</c>Model for REST API
    /// </summary>
    public class ClearInsightAPI
    {
        /// <summary>
        /// Instalce Variable<c>_baseUrl</c> api base url
        /// </summary>
        string _baseUrl = "192.168.1.108:3000";
        /// <summary>
        /// Instance Variable<c>_accessToken</c> access Token
        /// </summary>
        string _accessToken = "";

        /// <summary>
        /// Constructor <c>ClearInsigheAPI</c>
        /// </summary>
        /// <param name="baseUrl">api base url,like "www.cz-tek.com".</param>
        /// <param name="accessToken">api access token.</param>
        public ClearInsightAPI(string baseUrl, string accessToken)
        {
            _baseUrl = baseUrl;
            _accessToken = accessToken;
        }

        /// <summary>
        /// Execute RESTSharp Request
        /// </summary>
        /// <typeparam name="T">Delegator</typeparam>
        /// <param name="request">RestSharp.RestRequest request</param>
        /// <returns>Delegator</returns>
        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = _baseUrl;
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_accessToken, "Bearer");

            var response = client.Execute<T>(request);

            if(response.ErrorException != null)
            {
                throw new ClearInsightException(response.ErrorMessage);
            }
            return response.Data;
        }

        /// <summary>
        /// Execute RestSharp Request
        /// </summary>
        /// <param name="resquest">RestSharp.RestRequest request</param>
        /// <returns>CIResponse response</returns>
        public CIResponse Execute(RestRequest resquest)
        {
            var client = new RestClient();
            client.BaseUrl = _baseUrl;
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_accessToken, "Bearer");
            IRestResponse response;
            response = client.Execute(resquest);
            return _processStatusCode(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void ExecuteAsync(RestRequest request,Action<CIResponse> callback)
        {
            var client = new RestClient();
            client.BaseUrl = _baseUrl;
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_accessToken, "Bearer");
            client.ExecuteAsync(request, response =>
            {
                callback(_processStatusCode(response));
            });
        }

        /// <summary>
        /// Function <c>ImportkpiEntriesAsync</c>
        /// </summary>
        /// <param name="entry">KpiEntry</param>
        /// <param name="callback">Callback</param>
        public void ImportKpiEntriesAsync(KpiEntry entry, Action<CIResponse> callback)
        {
            KpiEntryValidator validator = new KpiEntryValidator();
            List<KpiEntry> lst = new List<KpiEntry>();
            lst.Add(entry);
            validator.validate(lst);

            var request = new RestRequest(Method.POST);
            request.Resource = "api/v1/kpi_entry/entry";

            request.AddParameter("email", entry.Email);
            request.AddParameter("kpi_id", entry.KpiID);
            request.AddParameter("date", entry.Date);
            request.AddParameter("value", entry.Value);

            ExecuteAsync(request, callback);
        }

        /// <summary>
        /// function ImportkpiEntriesAsync
        /// </summary>
        /// <param name="entries">array kpientries</param>
        /// <remarks>length of entries should not bigger than 500</remarks>
        /// <param name="callback">callback(CIResponse)</param>
        public void ImportKpiEntriesAsync(KpiEntry[] entries, Action<CIResponse> callback)
        {
            if (entries.Length > (int)CIRequest.MAXKPIENTRYCOUNT)
            {
                throw new CIRequestTooLong("Maximum count of kpi entries is" + CIRequest.MAXKPIENTRYCOUNT);
            }

            KpiEntryValidator validator = new KpiEntryValidator();
            validator.validate(entries.OfType<KpiEntry>().ToList());

            var request = new RestRequest(Method.POST);
            request.Resource = "api/v1/kpi_entry/entries";
            request.RequestFormat = DataFormat.Json;
            object[] objs = new object[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                objs[i] = new { kpi_id = entries[i].KpiID, date = entries[i].Date, value = entries[i].Value, email = entries[i].Email };
            }
            request.AddParameter("entries", request.JsonSerializer.Serialize(objs));
            ExecuteAsync(request,callback);
        }

        /// <summary>
        /// Upload one kpi
        /// </summary>
        /// <param name="entry">ClearInsight.Model.KpiEntry</param>
        /// <returns>CIResponse response</returns>
        public CIResponse ImportKpiEntries(KpiEntry entry) 
        {
            KpiEntryValidator validator = new KpiEntryValidator();
            List<KpiEntry> lst = new List<KpiEntry>();
            lst.Add(entry);
            validator.validate(lst);
            var request = new RestRequest(Method.POST);
            request.Resource = "api/v1/kpi_entry/entry";

            request.AddParameter("email", entry.Email);
            request.AddParameter("kpi_id", entry.KpiID);
            request.AddParameter("date", entry.Date);
            request.AddParameter("value", entry.Value);

            return Execute(request);
        }

        /// <summary>
        /// Upload bulk kpientries
        /// </summary>
        /// <param name="entries">Array of ClearInsight.Model.KpiEntry</param>
        /// <remarks>length of entries should not bigger than 500</remarks>
        /// <returns>CIResponse response</returns>
        public CIResponse ImportKpiEntries(KpiEntry[] entries)
        {
            if (entries.Length > (int)CIRequest.MAXKPIENTRYCOUNT)
            {
                throw new CIRequestTooLong("Maximum count of kpi entries is"+CIRequest.MAXKPIENTRYCOUNT);
            }
            KpiEntryValidator validator = new KpiEntryValidator();
            validator.validate(entries.OfType<KpiEntry>().ToList());

            var request = new RestRequest(Method.POST);
            request.Resource = "api/v1/kpi_entry/entries";
            request.RequestFormat = DataFormat.Json;
            object[] objs = new object[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                objs[i] = new { kpi_id = entries[i].KpiID, date = entries[i].Date, value = entries[i].Value, email = entries[i].Email };
            }
            request.AddParameter("entries", request.JsonSerializer.Serialize(objs));
            return Execute(request);
        }

        /// <summary>
        /// A Test function
        /// </summary>
        /// <returns></returns>
        public CIResponse TestSecret()
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "api/v1/kpi_entry/secret";

            return Execute(request);
        }

        /// <summary>
        /// process the server response
        /// </summary>
        /// <param name="response">RestSharp.IRestResponse</param>
        /// <returns>CIResponse response</returns>
        private CIResponse _processStatusCode(IRestResponse response)
        {
            int statusCode = (int)response.StatusCode;
            CIResponse res = new CIResponse();
            res.Code = statusCode;
            res.Content = response.Content;

            //check defined msg
            switch (statusCode)
            {
                case (int)CIResponseCode.ArgumentError:
                    throw new CIArgumentErrorException(res.Content);
                default:
                    break;
            }
            return res;
        }
    }
}
