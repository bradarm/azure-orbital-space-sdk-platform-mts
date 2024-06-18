using Microsoft.Azure.SpaceFx.MessageFormats.HostServices.Sensor;

namespace Microsoft.Azure.SpaceFx.PlatformServices.MessageTranslationService;

public partial class MessageHandler<T> {
    private void SensorsAvailableResponseHandler(MessageFormats.HostServices.Sensor.SensorsAvailableResponse? message, MessageFormats.Common.DirectToApp fullMessage) {
        if (message == null) return;
        using (var scope = _serviceProvider.CreateScope()) {
            _logger.LogInformation("Processing message type '{messageType}' from '{sourceApp}' (trackingId: '{trackingId}' / correlationId: '{correlationId}')", message.GetType().Name, fullMessage.SourceAppId, message.ResponseHeader.TrackingId, message.ResponseHeader.CorrelationId);

            MessageFormats.HostServices.Sensor.SensorsAvailableResponse? pluginResult =
                         _pluginLoader.CallPlugins<MessageFormats.HostServices.Sensor.SensorsAvailableResponse?, Plugins.PluginBase>(
                             orig_request: message,
                             pluginDelegate: _pluginDelegates.SensorsAvailableResponse);

            _logger.LogDebug("Plugins finished processing '{messageType}' (trackingId: '{trackingId}' / correlationId: '{correlationId}')", message.GetType().Name, message.ResponseHeader.TrackingId, message.ResponseHeader.CorrelationId);

            if (pluginResult == null) {
                _logger.LogInformation("Plugins nullified '{messageType}'.  Dropping Message (trackingId: '{trackingId}' / correlationId: '{correlationId}')", message.GetType().Name, message.ResponseHeader.TrackingId, message.ResponseHeader.CorrelationId);
                return;
            } else {
                _logger.LogInformation("Sending message '{messageType}' to '{appId}'  (trackingId: '{trackingId}' / correlationId: '{correlationId}')", message.GetType().Name, MessageFormats.Common.HostServices.Sensor, message.ResponseHeader.TrackingId, message.ResponseHeader.CorrelationId);
                _client.DirectToApp(appId: $"hostsvc-{MessageFormats.Common.HostServices.Sensor}", message: pluginResult);
            }
        };
    }
}
