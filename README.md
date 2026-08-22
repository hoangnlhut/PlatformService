# PlatformService
Build Platform Service in microservice architecture


• Building two .NET Microservices using the REST API pattern
• Working with dedicated persistence layers for both services
• Deploying our services to Kubernetes cluster
• Employing the API Gateway pattern to route to our services
• Building Synchronous messaging between services (HTTP & gRPC)
• Building Asynchronous messaging between services using an Event Bus (RabbitMQ)


1. Platform service
	- Create new entity model named Platform
	- Create db context for Platform service and configure it to use in-memory database first for development and SQL Server for production
	- Create a repository for Platform service to handle data access (interface and implementation)
	- Create Seed data for Platform service to populate the database with initial data
2. Command service
	- 
