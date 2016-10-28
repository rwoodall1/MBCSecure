using Core;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Web;
using System.IO;
using System.Net;
using ApiBindingModels;
using Newtonsoft.Json;
using Utilities;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Net.Http.Headers;
using Domain;
using Services;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Exceptionless;
using Exceptionless.Models;
namespace WebApp.Controllers.API {
    [RoutePrefix("api/state")]
    public class StateController : BaseApiController {

        [HttpGet]
        [Route("getStates")]
        public async Task<IHttpActionResult> GetStates()


        {
            
                var processingResult = new ServiceProcessingResult<List<StateBindingModel>> { IsSuccessful = true };
            try {
                var sqlQuery = "SELECT * FROM states order by name";
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@InvNumber","1") };
                var sqlQueryService = new SQLQuery();
                var stateResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
                if (!stateResult.IsSuccessful){
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error getting states", "Error getting ", true, false);
                    return Ok(processingResult);
                }

                if (stateResult.Data.Rows.Count > 0) {
                    List<StateBindingModel> States = new List<StateBindingModel>();
                    foreach(DataRow row in stateResult.Data.Rows)
                    {
                        StateBindingModel state = new StateBindingModel();                    
                        state.Abrv = row["abrv"].ToString();
                        state.Name = row["name"].ToString();
                        States.Add(state);
                    }
                    processingResult.Data =States;
                    processingResult.IsSuccessful = true;
                }
                else
                {
                    processingResult.IsSuccessful =false;
                    processingResult.Error = new ProcessingError("Failed to retrieve states", "Failed to retrieve states", true, false);
                    
                }

            } catch (Exception ex) {

                ex.ToExceptionless().Submit();
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Failed to retrieve states", "Failed to retrieve states", true, false);
            }
            return Ok(processingResult);
        }


       
       
    }
}