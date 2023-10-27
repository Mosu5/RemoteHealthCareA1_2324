using DoctorApp.Communication;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace RHSandbox.Communication
{
    /// <summary>
    /// Object for holding a request. If the server has responded to this request,
    /// then the Response attribute of this request is not null.
    /// </summary>
    public class Request
    {
        // The response sent by the server, or else null.
        private JsonObject _response;

        // An object that can block execution until its SetResult() method is called.
        // This is used for commands to let them wait for a response.
        private readonly TaskCompletionSource<JsonObject> _waiter;

        public readonly string Command;
        public readonly JsonObject Message;

        public Request(JsonObject message)
        {
            _waiter = new TaskCompletionSource<JsonObject>();
            (Command, Message) = DoctorProxy.GetCommandAndData(message);
        }

        /// <summary>
        /// Blocks execution until the response is received.
        /// </summary>
        /// <returns>The response sent by the server.</returns>
        public async Task<JsonObject> AwaitResponse()
        {
            return await _waiter.Task;
        }

        /// <summary>
        /// Once a response is received, by calling this method, any command using that specific
        /// request object will be able to resume execution, since a response has been received.
        /// </summary>
        /// <param name="response"></param>
        /// <exception cref="CommunicationException"></exception>
        public void SetResponse(JsonObject response)
        {
            // A response can only be set once per instance of this class.
            if (_response != null)
                throw new CommunicationException("Response was already set");

            // This will stop the blockage of the AwaitResponse() method and set its result to response.
            _waiter.SetResult(response);
            _response = response;
        }
    }
}
