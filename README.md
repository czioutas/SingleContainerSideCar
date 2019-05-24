## SingleContainerSideCar

The SingleContainerSideCar is a slimmed down version of full blown Service Mesh implementations like Istio and Envoy.

Small applications not running on Kuberneties still can take advantage of the sidecar/ambassador patterns, allowing the business logic 
application to remain even slimmer.

- Some of these features are:
- Service discovery
- Adaptive routing / client side load balancing
- Automatic retries
- Circuit breakers
- Timeout controls
- back pressure
- Rate limiting
- Metrics/stats collection
- Tracing
- request shadowing
- Service refactoring / request shadowing
- TLS between services
- Forced service isolation / outlier 

The current focus is on make this work as a Proxy with Telemetry so that we can remove any telemetry injection inside the business logic application.

## Structure

This project is a proof of concept, however further down, we could have the SideCar Solution as an independent project, while the SimpleApp
would be replaced by any application.

## How it works?

The SideCar works on Layer 7 and does not actually intercepts the call, rather than accepts and propagets.

Following is an example request
```
curl -X GET \
  http://localhost:5000/ \
  -H 'Accept: */*' \
  -H 'Cache-Control: no-cache' \
  -H 'Host: localhost:5000' \
  -H 'internal: false' \
  -H 'target: https://api.chucknorris.io/jokes/random'
```

As you can see the request is sent directly to the instance of the sidecar and provides two specific header values.
`internal` and `target`.

In order to keep things as simple as possible, the SideCar receives the request and propagates it to the request `target`. 
Based on the `internal` value it will do local service discovery or not.

## Going on

As mentioned before this is still a POC. More work needs to be done in terms of request propagation and telemetry.
Following that, heavy testing in terms of performance will be required to check if this is a viable solution and practicality.

For more find me on [twitter-czioutas](https://twitter.com/czioutas)
