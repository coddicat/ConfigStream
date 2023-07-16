using ConfigStream.Abstractions;
using System;
using System.Threading.Tasks;

namespace ConfigStream.Scope
{
    public class ConfigStreamScope : IConfigStreamScope
    {
        private readonly HandlerDelegate _readHandler;
        private readonly HandlerDelegate _writeHandler;
        internal ConfigStreamScope(HandlerDelegate readHandler, HandlerDelegate writeHandler, string scopeId, string environmentName)
        {
            _readHandler = readHandler;
            _writeHandler = writeHandler;
            
            ScopeId = scopeId;
            EnvironmentName = environmentName;
        }

        public string ScopeId { get; private set; }
        public string EnvironmentName { get; }

        public async Task<ConfigStreamValue> ReadAsync(string groupName, string configName, string targetName = null)
        {
            try
            {
                HandlerContext context = InitHandlerContext(groupName, configName, targetName);
                await _readHandler(context);
                return context.Value;
            }
            catch (Exception ex)
            {
                return new ConfigStreamValue
                {
                    Exception = ex,
                    IsSuccess = false,
                    Data = null
                };
            }
        }

        private HandlerContext InitHandlerContext(string groupName, string configName, string targetName)
        {
            return new HandlerContext
            {
                ScopeId = ScopeId,
                EnvironmentName = EnvironmentName,
                ConfigName = configName,
                GroupName = groupName,
                TargetName = targetName,
                Value = new ConfigStreamValue
                {
                    IsSuccess = false,
                    Data = null
                }
            };
        }

        public async Task WriteAsync(string value, string groupName, string configName, string targetName = null)
        {
            HandlerContext context = InitHandlerContext(groupName, configName, targetName);
            context.Value = new ConfigStreamValue { Data = value, IsSuccess = true };

            try
            {
                await _writeHandler(context);
            }
            catch (Exception)
            {
                //
            }            
        }
    }
}
