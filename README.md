# url-shortner
makes url shorter

## Development environment:
This solution makes use of CosmosDB install the emulator by following the instruction in [this page](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21)
the emulator can be [downloaded here](https://aka.ms/cosmosdb-emulator)

## Overall architecture

The system architecture is designed to scale horizontaly

It is composed by 
* an angluar client that can be served as static site
* a dot.net asp.net webapi backend which can be scale horizontally
* an high troughput persistence state (cosmos db) that can scale without limit

## Scalability concerns
Due to the nature of the problem, no transaction are required to satisfy the requirements, but optimistic concurrency is used to ensure consistency in a compination with retry policies.
A local cache is used to offload the database
The backend can be scaled horizontally with two different purposes
* put multiple backend on the same generation sequence
* multiple backend on multiple generation sequence

the client can be deployed as static site, both on the same origin of the server or on a different one.
for scalability it is foreseeble to deploy multiple client and multiple backend on differents nodes which can scale horizontally.
these nodes will be load balanced so to appear to be the same resource to the caller.

## working principle
when a new short url is required, a sequencialnumber is obtained from a sequence, each worked pre allocate chunk of these sequence and then interlally assigns a globally unique number to each request.
the number is then encoded in a special base-16 format using letters only.
thisis then prefixed with the name of the generating sequence.
this can be usefull both for surpass the limit of 64bit (lol) of each sequence, but more importantly to partition, segregate, and eventually scale the backend, as each backend operates only on one sequence each.
So, the encoded sequential id together with the prefix of the sequence name is used as slug and recorded in the database together with the "long url" provided by the user, in a document identified by the slug name itself.

when a request for a slug arrives, the database is searched for that slug, if found a redirect response is issued.
the system uses normal redirect, non permanent redirec, in case in a future will be required to count the number of redirect to create metrics.

searches for short url on the database are always preceeded by search on the local cache for such information, so to offload the database of a trivial search.

## Security
No security is implemented, the service is publicly avaiable. CORS are implemented to limit the usage to legit clients, for what is worth.





