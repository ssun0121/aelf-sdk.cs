namespace AElf.Client;

public sealed class AElfClientBuilder
{
    private string NodeEndpoint { get; set; }
    private int Timeout { get; set; }

    private string UserName { get; set; }
    private string Password { get; set; }

    public AElfClientBuilder()
    {
        NodeEndpoint = AElfClientConstants.LocalEndpoint;
        Timeout = 60;
    }

    public AElfClientBuilder UseEndpoint(string endpoint)
    {
        NodeEndpoint = endpoint;
        return this;
    }

    public AElfClientBuilder UsePublicEndpoint(EndpointType endpointType)
    {
        switch (endpointType)
        {
            case EndpointType.MainNetMainChain:
                NodeEndpoint = AElfClientConstants.MainNetMainChain;
                break;
            case EndpointType.MainNetSidechain:
                NodeEndpoint = AElfClientConstants.MainNetSidechain;
                break;
            case EndpointType.TestNetMainChain:
                NodeEndpoint = AElfClientConstants.TestNetMainChain;
                break;
            case EndpointType.TestNetSidechain:
                NodeEndpoint = AElfClientConstants.TestNetSidechain;
                break;
            case EndpointType.Local:
            default:
                NodeEndpoint = AElfClientConstants.LocalEndpoint;
                break;
        }

        return this;
    }

    public AElfClientBuilder SetHttpTimeout(int timeout)
    {
        Timeout = timeout;
        return this;
    }

    public AElfClientBuilder ManagePeerInfo(string userName, string password)
    {
        UserName = userName;
        Password = password;
        return this;
    }

    public AElfClient Build()
    {
        return new AElfClient(NodeEndpoint, Timeout, UserName, Password);
    }
}