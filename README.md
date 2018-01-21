# Cloud driver supervisor
This is a repository for a C#/Angular/Android solution, that was developed during the creation of a masters thesis.

Master’s thesis solution shows, how to create a cloud solution, which is economically more favorable, then a locally deployed solution inside the company’s local computer network.
The solution is developed for lorry management, because logistical systems need a more economically favorable, different load resistant and scalable cloud solution.

## Overall architecture
![Overall_Architecture](https://github.com/Dimarto/cloud-driver-supervisor/raw/master/docs/images/Overall_architecture.png)

The solution is deployed on Microsoft Azure and consists of different type of resources:
- Azure service fabric - the base for the business functionality, that is ran on microservices;
- 4 microservices that are responsible for a specific domain functionality (managing GPS locations etc.);
- Service bus - used as an communication layer between parts of the system;
- Neo4j high availability cluster - provides the data layer functionality;
- Angular web application - used to manage lorry driver actions;
- Android mobile application - used by lorry drivers to send the current GPS locations;
- Azure active directory - stores users, that can authenticate in the system.

### Performance test results
582413 requests were created during the 17 minute long performance test. The test results show, how at 23:15:40 cloud autoscaling functionality helps to handle the huge amount requests.

![Active_message_count](https://github.com/Dimarto/cloud-driver-supervisor/raw/master/docs/images/Active_message_count.png)

The growth of incoming requests count was shrinking when multiple microservices were handling the incoming messages.

![Active_message_count_growth_decrease](https://github.com/Dimarto/cloud-driver-supervisor/raw/master/docs/images/Active_message_count_growth_10_seconds.png)

The average response time was **159** ms and the throughput was **477.72** requests per second.

### GUI
Web application:

![Web_app](https://github.com/Dimarto/cloud-driver-supervisor/blob/master/docs/images/Web_app.JPG?raw=true)

Mobile application:

![Mobile_app](https://github.com/Dimarto/cloud-driver-supervisor/raw/master/docs/images/Mobile_app.png)
