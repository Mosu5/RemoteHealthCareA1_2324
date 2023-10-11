﻿using DoctorApp.Communication;
using DoctorApp.Helpers;
using RHSandbox.Communication;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace DoctorApp.Commands
{
    internal class SessionPause : IDoctorCommand
    {
        private readonly string _patientUsername;

        public SessionPause(string patientUsername)
        {
            _patientUsername = patientUsername;
        }

        public async Task<bool> Execute()
        {
            Request request = new Request(DoctorFormat.SessionPauseMessage(_patientUsername));
            JsonObject response = await DoctorProxy.GetResponse(request);

            if (!response.ContainsKey("status"))
                throw new CommunicationException("The login message did not contain the JSON key 'status'");

            if (!response["status"].ToString().Equals("ok"))
                return false;

            return true;
        }
    }
}