using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Core;
using Core.Interfaces;
using Utilities;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Exceptionless;

namespace Services {
    public class SQLQuery {
       
        private static string _ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public SQLQuery() {
        }

        public async Task<ServiceProcessingResult<int>> ExecuteNonQueryAsync(CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) {
            var result = new ServiceProcessingResult<int> { IsSuccessful = true };

            using (var connection = new MySqlConnection(_ConnectionString)) {
                using (var command = new MySqlCommand(cmdText, connection)) {
                    try {
                        command.CommandType = cmdType;
                        command.Parameters.Clear();
                        command.Parameters.AddRange(commandParameters);
                        connection.Open();

                        var sqlResult = await command.ExecuteNonQueryAsync();
                        result.Data = sqlResult;
                    } catch (Exception ex) {
                        result.IsSuccessful = false;
                        ex.ToExceptionless()
                           .SetMessage("Error running ExecuteNonQueryAsync.")
                           .AddTags("Sql Query Wrapper")
                           .AddObject(cmdText, "Query")
                           .AddObject(CommandParametersToObj(commandParameters), "Query Parameters")
                           .MarkAsCritical()
                           .Submit();
                        result.Error = new ProcessingError("Error processing ExecuteNonQueryAsync", "Error processing ExecuteNonQueryAsync", true, false);
                    } finally {
                        connection.Close();
                    }

                    return result;
                }
            }

        }

        public async Task<ServiceProcessingResult<DataTable>> ExecuteReaderAsync(CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) {
            return await ExecuteReaderAsync(cmdType, cmdText, 10, commandParameters);
        }

        public async Task<ServiceProcessingResult<object>> ExecuteReaderAsync<T>(CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) {
            return await ExecuteReaderAsync<T>(cmdType, cmdText,10, commandParameters);
        }

        public async Task<ServiceProcessingResult<object>> ExecuteReaderAsync<T>(CommandType cmdType, string cmdText, int TimeoutInSeconds, params MySqlParameter[] commandParameters) {

            var processingResult = new ServiceProcessingResult<object> { IsSuccessful = true };

            using (var connection = new MySqlConnection(_ConnectionString)) {
                using (var command = new MySqlCommand(cmdText, connection)) {
                    try {
                        var dt = new DataTable();
                        command.CommandType = cmdType;
                        command.Parameters.Clear();
                        command.Parameters.AddRange(commandParameters);
                        command.CommandTimeout = TimeoutInSeconds;
                        connection.Open();

                        using (MySqlDataAdapter a = new MySqlDataAdapter(command)) {
                            a.Fill(dt);
                        }

                        if (dt.Rows.Count < 1) {
                            processingResult.Data = null;
                            return processingResult;
                        }
                        try {
                            var Vals = CollectionHelper.ConvertTo<T>(dt);
                            processingResult.Data = Vals;
                        } catch (Exception ex) {
                            processingResult.IsSuccessful = false;
                            processingResult.Error = new ProcessingError("Error retrieving data.", "Error retrieving data.", true, false);

                            ex.ToExceptionless()
                               .SetMessage("Collection Helper Error")
                               .AddTags("Collection Helper")
                               .MarkAsCritical()
                               .Submit();
                        }

                        return processingResult;
                    } catch (Exception ex) {
                        processingResult.IsSuccessful = false;
                       
                        ex.ToExceptionless()
                           .SetMessage("Error running ExecuteReaderAsync(object).")
                           .AddTags("Sql Query Wrapper")
                           .AddObject(cmdText, "Query")
                           .AddObject(CommandParametersToObj(commandParameters), "Query Parameters")
                           .MarkAsCritical()
                           .Submit();
                        processingResult.Error = new ProcessingError("Error running ExecuteReaderAsync. Ex: " + ex.ToString(), "Error running ExecuteReaderAsync. Ex: " + ex.ToString(), false, false);
                        return processingResult;
                    } finally {
                        connection.Close();
                    }
                }
            }
        }

        public async Task<ServiceProcessingResult<DataTable>> ExecuteReaderAsync(CommandType cmdType, string cmdText, int TimeoutInSeconds, params MySqlParameter[] commandParameters) {
            var processingResult = new ServiceProcessingResult<DataTable> { IsSuccessful = true };

            using (var connection = new MySqlConnection(_ConnectionString)) {
                using (var command = new MySqlCommand(cmdText, connection)) {
                    try {
                        var dt = new DataTable();
                        command.CommandType = cmdType;
                        command.Parameters.Clear();
                        command.Parameters.AddRange(commandParameters);
                        command.CommandTimeout = TimeoutInSeconds;
                        connection.Open();

                        using (MySqlDataAdapter a = new MySqlDataAdapter(command)) {
                            a.Fill(dt);
                        }
                        processingResult.Data = dt;
                        return processingResult;
                    } catch (Exception ex) {
                        processingResult.IsSuccessful = false;
                        ex.ToExceptionless()
                          .SetMessage("Error running ExecuteReaderAsync(datatable).")
                          .AddTags("SqlQueryWrapper")
                          .AddObject(cmdText, "Query")
                          .AddObject(CommandParametersToObj(commandParameters), "Query Parameters")
                          .MarkAsCritical()
                          .Submit();

                        processingResult.Error = new ProcessingError("Error running ExecuteReaderAsync. Ex: " + ex.ToString(), "Error running ExecuteReaderAsync. Ex: " + ex.ToString(), false, false);
                        return processingResult;
                    } finally {
                        connection.Close();
                    }
                }
            }
        }

        public async Task<ServiceProcessingResult<object>> ExecuteScalarAsync(CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) {
            var result = new ServiceProcessingResult<object> { IsSuccessful = true };
            using (var connection = new MySqlConnection(_ConnectionString)) {
                using (var command = new MySqlCommand(cmdText, connection)) {
                    try {
                        command.CommandType = cmdType;
                        command.Parameters.Clear();
                        command.Parameters.AddRange(commandParameters);
                        connection.Open();

                        var sqlResult = await command.ExecuteScalarAsync();
                        result.Data = sqlResult;
                    } catch (Exception ex) {
                        result.IsSuccessful = false;
                        ex.ToExceptionless()
                          .SetMessage("Error running ExecuteScalarAsync.")
                          .AddTags("SqlQueryWrapper")
                          .AddObject(cmdText, "Query")
                          .AddObject(CommandParametersToObj(commandParameters), "Query Parameters")
                          .MarkAsCritical()
                          .Submit();

                        result.Error = new ProcessingError("Error processing ExecuteScalarAsync", "Error processing ExecuteScalarAsync", true, false);
                    } finally {
                        connection.Close();
                    }

                    return result;
                }
            }
        }

        protected string GetFormattedGenericLogMessage(string logMessage) {
            return String.Format(logMessage);
        }
        protected List<paramObj> CommandParametersToObj(MySqlParameter[] commandParameters) {
            List<paramObj> vParams = new List<paramObj>();
            foreach (var param in commandParameters) {
                paramObj item = new paramObj();
                item.Value = param.Value == null ? "null" : param.Value.ToString();
                item.Name = param.ParameterName.ToString();
                vParams.Add(item);
            }
            return vParams;
        }
    }
    public class paramObj {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
