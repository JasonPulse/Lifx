using System;
using System.Net;
using System.Threading.Tasks;
using Lifx.Communication;
using Lifx.Communication.Requests;
using Lifx.Communication.Responses;
using Lifx.Communication.Responses.Payloads;

namespace Lifx
{
	public sealed class LightFactory
	{
		private const int Port = 56700;

		private static readonly TimeSpan ResponseExpiry = TimeSpan.FromSeconds(5);
		private static readonly IResponseParser ResponseParser = new ResponseParser(
			new StateVersionResponsePayloadParser(),
			new StateResponsePayloadParser()
		);

		public async Task<ILight> CreateLightAsync(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException(nameof(address));
			}

			var endPoint = new IPEndPoint(address, Port);
			var communicator = new Communicator(ResponseParser, endPoint, ResponseExpiry);
			var requestFactory = new RequestFactory();

			var request = requestFactory.CreateGetVersionRequest();
			var payload = await communicator.CommunicateAsync<StateVersionResponsePayload>(request);

			return new Light(address, payload.Product, payload.Version, communicator, requestFactory);
		}
	}
}