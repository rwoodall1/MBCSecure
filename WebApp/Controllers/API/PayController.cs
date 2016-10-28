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

namespace WebApp.Controllers.API {
    [RoutePrefix("api/invoice")]
    public class InvoiceController : BaseApiController {

        [HttpGet]
        [Route("invoiceCodeExist")]
        public async Task<IHttpActionResult> InvoiceCodeExist(string invNumber) {
            var processingResult = new ServiceProcessingResult<bool> { IsSuccessful = true };
            try {
                var sqlQuery = @"SELECT Invno FROM InvoiceInfo WHERE Password=@InvNumber";
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@InvNumber", invNumber) };
                var sqlQueryService = new SQLQuery();
                var getInoviceCodeResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
                if (!getInoviceCodeResult.IsSuccessful){
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error getting invoice code", "Error getting invoice code", true, false);
                    return Ok(processingResult);
                }

                if (getInoviceCodeResult.Data.Rows.Count > 0) { processingResult.Data = true; } else { processingResult.Data = false; }

            } catch (Exception ex) {
                ex.ToExceptionless().Submit();
            }
            return Ok(processingResult);
        }


        [HttpGet]
        [Route("invoiceInit")]
        public async Task<IHttpActionResult> InvoiceInit (string invNumber)
        {
            var processingResult = new ServiceProcessingResult<InvoiceInitBindingModel> { IsSuccessful = true };
            try
            {
                var sqlQuery = @"SELECT I.schcode, I.invno, D.teacher, D.id FROM invoiceinfo I LEFT JOIN dropdowninfo D ON I.schcode=D.schcode  WHERE I.invno=@InvNumber";
                MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("@InvNumber", invNumber) };
                var sqlQueryService = new SQLQuery();
                var getInvoiceTeacherLookupResult = await sqlQueryService.ExecuteReaderAsync<InvoiceTeacherLookupBindingModel>(CommandType.Text, sqlQuery, parameters);
                if (!getInvoiceTeacherLookupResult.IsSuccessful)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error getting teachers", "Error getting teachers", true, false);
                    ExceptionlessClient.Default.SubmitLog("Error getting teachers");
                    return Ok(processingResult);
                }

                sqlQuery = @"SELECT I.schcode, D.grade, D.id FROM invoiceinfo I LEFT JOIN dropdowninfo D ON I.schcode = D.schcode  WHERE I.invno=@InvNumber";
                parameters = new MySqlParameter[] { new MySqlParameter("@InvNumber", invNumber) };
                var getInvoiceGradeLookupResult = await sqlQueryService.ExecuteReaderAsync<InvoiceGradeLookupBindingModel>(CommandType.Text, sqlQuery, parameters);
                if (!getInvoiceGradeLookupResult.IsSuccessful)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error getting grades", "Error getting grades", true, false);
                    ExceptionlessClient.Default.SubmitLog("Error getting grades");
                    return Ok(processingResult);
                }

                sqlQuery = @"SELECT id, cvalue, isortorder, caption, ivalue, csortorder FROM Lookup";
                var getInvoiceIconLookupResult = await sqlQueryService.ExecuteReaderAsync<InvoiceIconLookupBindingModel>(CommandType.Text, sqlQuery, parameters);
                if (!getInvoiceIconLookupResult.IsSuccessful)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error getting icons", "Error getting icons", true, false);
                    ExceptionlessClient.Default.SubmitLog("Error getting icons");
                    return Ok(processingResult);
                }

                sqlQuery = @"SELECT schname AS schoolname FROM invoiceinfo WHERE invno=@InvNumber";
                parameters = new MySqlParameter[] { new MySqlParameter("@InvNumber", invNumber) };
                var getSchoolNameResult = await sqlQueryService.ExecuteReaderAsync<InvoiceSchoolNameBindingModel>(CommandType.Text, sqlQuery, parameters);
                if (!getSchoolNameResult.IsSuccessful)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error getting school name", "Error getting school name", true, false);
                    ExceptionlessClient.Default.SubmitLog("Error getting school name");
                    return Ok(processingResult);
                }


                processingResult.IsSuccessful = true;

                var schoolname = (List<InvoiceSchoolNameBindingModel>)getSchoolNameResult.Data;

                processingResult.Data = new InvoiceInitBindingModel();
                processingResult.Data.SchoolName = schoolname[0].schoolname;
                processingResult.Data.Teachers = (List<InvoiceTeacherLookupBindingModel>) getInvoiceTeacherLookupResult.Data;
                processingResult.Data.Grades = (List<InvoiceGradeLookupBindingModel>) getInvoiceGradeLookupResult.Data;
                processingResult.Data.Icons = (List<InvoiceIconLookupBindingModel>) getInvoiceIconLookupResult.Data;

            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
            }
            return Ok(processingResult);
        }
    }
}