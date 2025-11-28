#cloud #aws

## Module 1 Introduction to the Cloud

What does it mean to have "On-demand Delivery" in a cloud context?::It means the customer can access computing resources such as storage, compute power as needed.

What are the 3 deployment models for cloud resources?::1) Cloud, 2) On-premise deployment, 3) Hybrid deployment

What is the benefit of the cloud-based deployment?::The user has the flexibility to migrate the existing resources to the cloud, design and build new applications within the cloud environment, or use a combination of both

What is the benefit of on-premise deployment?::It can provide dedicated resources and low latency

What is the benefit of a hybrid deployment approach to cloud services?::This approach is beneficial when legacy applications must remain on premises due to maintenance preferences or regulatory requirements.

What are the 6 benefits of the cloud?::1) the ability to pay as you go, 2) the massive economy of scale, 3) scaling capacity is easy, 4) increase speed and agility to experiment, 5) no upfront costs of maintaining a data center or servers, 6) the ability to deploy your applications to different regions easily

What are AWS Regions?::AWS Regions are physical locations around the world that contain groups of data centers

What is an availability zone with respect to AWS?::An availability zone is a group of data centers

With respect to AWS, at a minimum, how many availability zones does a AWS region consist of within a geographic area?::At minimum, it contains 3 regions

What is the purpose of an availability zone with respect to AWS?::The purpose of an availability zone is to provide users within an area with low-latency, fault tolerant access to services

What does an availability zone consist of with regards to AWS?::It consists of one or more data centers with redundant power, networking, and connectivity

What does AZ stand for with regards to AWS data centers?::It stands for availability zones

What is AWS's shared responsibility principle?::It is the concept that AWS is responsible for the security of the cloud while the customer is responsible for security in the cloud

What are some of the customer's responsibility with regards to the AWS shared responsibility principle?::The customer is responsible for the customer data and client-side data encryption

What are some responsibilities with regards to AWS shared responsibility principle that might fall under the customer or AWS depending on the web service?::The server side encryption, network traffic protection, platform and application management, and OS, network, and firewall configuration

What are some of AWS's responsibilities with regards to AWS shared responsibility principle?::The software for compute, storage, database, and networking as well as the hardware and the AWS global infrastructure

## Module 2 - Compute in the Cloud

What does EC2 stand for with regards to AWS?::It refers to Amazon Elastic Compute Cloud

With EC2, which services are you paying for?::You are only paying for running instances, not stopped or terminated ones

What is the concept of multi-tenancy?::The concept of multi-tenancy is where you make sure each virtual machine is isolated from each other but is still able to share resources provided by the host

What is the purpose of a hyper-visor?::The job of a hyper-visor is to handle resource sharing and isolation

What does it mean to vertically scale an instance?::To vertically scale an instance is to provide more resources like memory or compute to that instance

What does AMI stand for with regards to AWS?::It stands for Amazon Machine Image

What does an AMI with regards to AWS define?::It defines the operating system which might include additional software as well

What is the high level process for launching an EC2 instance?::First, you select an Amazon machine image, you choose an instance type, then you connect to your EC2 instance, then you can run the required commands to install software, add storage, organize files and perform other tasks

What are the 5 types of EC2 instances provided?::General purpose instances, compute-optimized instances, memory-optimized instances, accelerated-computing instances and storage-optimized instances

What does a general purpose EC2 instance provide?::It provides a good balance of compute, memory, and networking resources

What does a compute-optimized EC2 instance provide?::It provides high performance for tasks like gaming servers, high-performance computing, machine learning tasks, and even scientific modelling

What does a memory-optimized EC2 instances provide?::It provides fast performance for workloads that process large data sets in memory like large dataset, data analytics and databases

What does an accelerated-optimized EC2 instance provide?::It provides performance for floating point number calculations, graphics processing, or data pattern matching

What does a storage-optimized EC2 instance provide?::It provides high performance for locally stored data for workloads like large databases, data warehousing and I/O-intensive applications

What are the 3 ways to call AWS?::You can call AWS using the AWS Management Console, the AWS Command Line Interface, or the the AWS Software Development Kit

How does an end user interact with the AWS Management Console?::It relies on a user pointing and clicking through various screens

How does an end user interact with the AWS Command Line Interface?::It relies on a user providing text based commands making automation through scripting possible

What command should be used to list all AZs in the current region?::"aws ec2 describe-availability-zones"

What command should be used to create an EC2 instance programmatically?::"aws ec2 run-instance"

What does the AWS SDK provide?::It simplifies integrating AWS services into your application by providing APIs for various programming languages

What are some possible triggers for Amazon Lambda?::It can be from events like Amazon services, mobile apps, or HTTP requests

What are some examples of 3 tasks that fall under the customer's responsibility for securing applications in the cloud?::The customer is responsible for configuring security, managing the guest operating system (OS), applying updates, and setting up firewalls (security groups)

What is a key-pair in the context of AWS?::A key pair refers to a public and private key which is how we log into EC2 instances

What are the 3 required configurations when launching a Amazon EC2 instance for a web server?::An AMI, the instance type, and the storage

What are the 5 items an AMI comprise of?::1) An AMI is comprised of the operating system, 2) storage setup, 3) architecture type, 4) permissions for launching, and 5) any extra software that is already installed

What are the 3 ways that AMIs can be obtained?::1) you can create your own image, 2) you can use AWS AMIs, 3) you can purchase AMIs from the AWS marketplace

What are the 5 pricing options for EC2 usage?::1) On-Demand, 2) Savings Plan, 3) Reserved Instances, 4) Spot Instances, 5) Dedicated Hosts

What is the AWS policy on the pricing for On-Demand instances?::Pay only for the compute capacity you consume with no upfront payments or long-term commitments

What is the AWS policy on the pricing for Savings Plans?::Commit to terms of 1-year or 3-years and get savings up to 72% savings

What is the AWS policy on the pricing for Reserved Instances?::Commit to terms of 1-year or 3-years for predictable workloads using specific instance families and AWS regions and get up to 75% savings

What is the AWS policy on the pricing for Dedicated Hosts?::Reserve an entire physical server for your exclusive use. This option offers full control which is ideal for workloads with strict security or licensing needs

What is the AWS policy on the pricing for Spot Instances?::Bid on spare compute capacity at up to 90% savings with the flexibility to be interrupted when AWS reclaims the instance

What is the AWS policy on the pricing for Dedicated Instances?::Pay in advance for hardware dedicated solely to your account. This option provides isolation from other AWS customers

What is the difference between Dedicated Hosts and Dedicated Instances?::With Dedicated Hosts, you get exclusive use of the servers while with Dedicated Instances you do not get to choose the physical servers that you run on but you are provided the entire server for your exclusive use with the same isolation

What is the AWS Savings Plan pricing policy good for?::It is good for predictable workloads where you specify an amount of compute power required over a 1-year or 3-year period

What are the 2 flavors of reserved instances with AWS Reserved pricing?::Reserved Capacity and Reserved Instances

What is the AWS Reserved Capacity pricing policy good for?::It is good for critical workloads with strict capacity requirements

What is the AWS Reserved Instance pricing policy good for?::It is good for steady-state workloads with predictable usage

What is the cloud concept of scalability?::It is the ability to handle an increased load by adding resources either by increasing the power that a machine has or adding more machines.

What is the cloud concept of elasticity?::It is the ability to automatically scale resources up or down in response to real-time demand. Elasticity provides cost efficiency

What are the two directions of scaling to handle growing demands in cloud services?::Scaling out AKA horizontal scaling or Scaling up AKA vertical scaling

What does Scaling out do?::It increases the number of instances that are deployed such that more work can be done in parallel

What does Scaling up do?::It increases the power per instance such that each instance can perform the task faster

What are the two approaches to Amazon EC2 Auto Scaling?::Dynamic scaling and predictive scaling

What does Amazon EC2 Dynamic Scaling do?::It adjusts in real time to fluctuations in demand

What does Amazon EC2 Predictive Scaling do?::It preemptively schedules the right number of instances based on anticipated demand

What are Auto Scaling groups with regards to Amazon EC2?::They are collections of EC2 instances that can scale in or out to meet your application needs

What are the 3 parameters to set with regards to Amazon EC2 capacity with Auto Scaling?::The minimum capacity, the desired capacity and the maximum capacity

What does the desired capacity parameter in Amazon EC2 Auto Scaling set?::It sets the ideal number of instances to handle the current workload

What is the main reason for deploying Amazon EC2 instances across multiple Availability Zones?::To provide high availability by allowing instances in different Availability Zones to handle traffic if one Availability Zone fails

What does ELB stand for with regards to AWS?::It stands for Elastic Load Balancing

What does the Elastic Load Balancing in AWS do?::It distributes incoming application traffic across multiple resources such as EC2 instances to optimize performance and reliability

What are the 3 main benefits of Elastic Load Balancing?::Efficient traffic distribution, automatic scaling, and simplified management

What does the benefit of ELB's efficient traffic distribution provide?::It evenly distributes traffic across EC2 instances, preventing overload on any single instance

What does the benefit of ELB's auto scaling provide?::It scales traffic automatically in response to demand

How does ELB provide simplified management?::It provides simplified management by decoupling the front-end and backend tiers and reduces manual synchronization

What are the 4 routing methods provided with ELB?::1) Round Robin, 2) Least Connections, 3) IP Hash, 4) Least Response Time

What does the Round Robin ELB routing method do?::It distributes traffic evenly across multiple available servers in a cyclic manner

What does the Least Connections ELB routing method do?::It routes traffic to the server with the fewest active connections

What does the IP Hash ELB routing method do?::It uses the client IP address to consistently route traffic to the same server

What does the Least Response Time routing method do?::It directs traffic to the server with the fastest response time, minimizing latency

What are the 3 services that AWS provides to handle Messaging and Queuing?::1) The Amazon Simple Queue Service (Amazon SQS), 2) the Amazon Simple Notification Service (Amazon SNS), and 3) the Amazon EventBridge

What does the Amazon SQS provide?::It provides the ability to send, store and receive messages between software components at any volume without losing messages or requiring other message consumers to be available

What does the Amazon SNS provide?::It provides the ability to send messages to services without any queuing and requires an immediate response like fanning out a notification to all end users

What is the benefit of a loosely coupled microservice architecture?::The failure of a single system does not detriment the entire system

What is the purpose of the Amazon EventBridge?::It is a serverless service that helps connect different parts of an application using events to help build a scalable, event-driven system by routing events to relevant services

How does the Amazon EventBridge handle the case where a listener of an event fails an is unable to receive an event?::The EventBridge will store the event and process it as soon as the service is available again

## Module 3: Introduction to Serverless Computing

What does Amazon ECS stand for?::It stands for Elastic Container Service

What does Amazon EKS stand for?::It stands for Elastic Kubernetes Service

What is an unmanaged service with respect to AWS?::An unmanaged service provides virtual machines as a service where the customer controls the patching, scaling and managing the operating system while Amazon takes care of the underlying infrastructure

What is a managed service with respect to AWS?::Managed services that have been implemented by AWS where you can configure the services to meet your requirements and AWS ensures that it runs smoothly over time with no server management required on our part

What does serverless mean with respect to AWS?::It means that the customer of AWS does not actually see or access the underlying infrastructure, scaling, high availability or the maintenance but instead just focuses on the application

With an Amazon offering like Amazon Lambda, what is the customer responsible for?::The customer is responsible for securing and managing the application code

With an Amazon offering like Amazon Lambda, what will AWS take care of?::AWS will take care of the infrastructure, scaling, and availability

What does Amazon Lambda provide developer?::It provides an environment to run code in response to an event without needing the developer to provision or manage servers

What are some possible triggers for Amazon Lambda?::It can be from events like Amazon services, mobile apps, or HTTP requests

What does a container solve the problem of portability of the code?::It packages everything the application code needs to run, the code, runtime, dependencies, and configuration into a single portable unit so as to create a consistent environment for the application to run in

What is the purpose of Amazon Elastic Container Registry?::The Amazon ECR is a fully managed container registry that stores your container images

What are the two options on AWS for hosting containers?::You can host your containers on Amazon EC2 or AWS Fargate

What is the difference between a container and a virtual machine in terms of the operating system that runs both?::With a container, all containers on the same server share the same host operating system where with virtual machines the hypervisor manages running several virtual machines where each virtual machine runs a separate full operating system

What are the 3 categories of tools related to AWS for managing containers?::Orchestration tools, registry tools and compute tools

What is Amazon ECS akin to in terms of widely available cloud tools?::It is akin to docker

When should one use Amazon ECS with Amazon EC2?::It should be used for small to medium sized businesses that needs full control over the infrastructure or when custom applications require specific hardware or networking configurations

When should one use Amazon ECS with Amazon Fargate?::It should be used with startups or small teams that need to build web applications with variable traffic

When should one use Amazon EKS with Amazon EC2?::It should be used for enterprises needing full control over infrastructure along with deep customization of EC2 instances alongside kubernetest scalability

When should one use Amazon EKS with Amazon Fargate?::It should be used for teams that want the kubernetes flexibility without needing to manage servers. It combines the power of kubernetes with the serverless simplicity

What does OCI stand for with respect to containers?::It stands for Open Container Initiative

What is the purpose of AWS Elastic Beanstalk?::It is a fully managed service that streamlines the deployment, management and scaling of web applications

What are the 4 things that AWS Elastic Beanstalk takes care of?::It takes care of 1) provisioning of the infrastructure, 2) scaling, 3) load balancing, and 4) application health monitoring

What is the target use case of AWS Beanstalk?::It is good for deploying and managing web applications, RESTful APIs, mobile backend services and microservice architectures

What is the purpose of AWS Batch?::AWS Batch is a fully managed service that you can use to run batch computing workloads on AWS. It automates the schedules, manages and scales compute resources for batch jobs

What is the target usecase for AWS Batch?::It is for processing large-scale, parallel workloads in areas like scientific computing, financial risk analysis, machine learning training and genomics research

What does VPF stand for with regards to AWS Lightsail?::It stands for virtual private servers

What is the purpose of AWS Lightsail?::AWS Lightsail is a cloud service that offers virtual private servers, storage, databases, and networking at a predictable monthly price

What are the 5 target use cases for AWS Lightsail beneficial for?::It is beneficial for 10 small businesses, 2) basic workloads, 3) low-traffic websites and 40 development and 5) testing environments

What is the purpose of AWS Outposts?::It provides a fully managed hybrid cloud solution that extends the AWS infrastructure and services onto on-premises data centers to provide a consistent experience between on-premises and the AWS cloud

## Module 4 - Introduction to Going Global

What are edge locations with regards to AWS?::Edge locations are smaller foot print facilities that are used to cache items like images, videos and other resources so that users can access the content they need with lower latency

What is the purpose of the AWS service CloudFormation?::The service helps to automate the deployment of cloud resources using Infrastructure as code to help achieve consistent, reliable set up each time

What does IaC stand for?::It stands for Infrastructure as Code

Is it possible for data to flow between AWS Regions without explicitly granting permission for that data to be moved?::No, data cannot flow between AWS Regions without explicit permissions being granted

What are the 4 considerations to be made when choosing an AWS Region?::1) compliance, 2) proximity, 3) feature availability, 4) pricing

What is encompassed in the consideration of compliance when choosing an AWS Region?::Different geographic locations have different regulatory requirements and data protection laws and the applications deployed to that AWS Region will have to comply with those laws

What is encompassed in the consideration of proximity when choosing an AWS Region?::Regions closer to the user base will minimize data travel time, which reduces latency and enhances application responsiveness

What is encompassed in the consideration of feature availability when choosing an AWS Region?::AWS is constantly expanding new features and services but not all AWS Regions contain all the newest features due to regional restrictions from compliance and security requirements of that region

What is encompassed in the consideration of pricing when choosing an AWS Region?::Some region has lower operational costs than others

What does the advantage of high availability in a cloud environment mean?::It refers to the capability of a system to operate continuously without failing. It means that the deployed application can handle failure of individual components without significant downtime

What does the advantage of agility in a cloud environment mean?::It refers to the ability to quickly adapt to changing requirements or market conditions

What does the advantage of elasticity in a cloud environment mean?::It refers to the ability of a system to automatically scale resources up or down automatically in response to changes in demand

What is AWS CloudFront?::AWS CloudFront is Amazon's content delivery network (CDN)

What is a CloudFormation template?::A CloudFormation template is a text-based document where you can specify what you want to build without specifying the details of exactly how to build it

## Module 5 - Networking

What does the term Networking consist of with regards to AWS Cloud?::It consists of the infrastructure and services working together to host your application, data, and any other resources you might need

What does VPC stand for with regards to AWS Cloud?::It refers to virtual private cloud

What does a VPC allow you to do within AWS Cloud?::It allows you to provision a logically isolated section of the AWS Cloud where you can launch AWS resources in a virtual network that you define

What is a subnet with respect to AWS Cloud?::It is a subsection of a VPN that can either be public or private. It is usually represented as a range of IP addresses

What is required in AWS Cloud to allow public internet traffic to flow into and out of your VPC?::You must attach an internet gateway

What should be used to only allow specific connection access to a private VPC?::A virtual private gateway should be used

When having a virtual private gateway and a internet gateway, do they both have isolated networks or shared networks?::Both gateways when within the same VPC use the same shared network

What does AWS Direct Connect service provide?::It provides a completely private, dedicated fiber connection from your data center to AWS with ensured security and consistent high performance

What are the 3 benefits of AWS VPC?::1) Increased security through network isolation of components, 2) reduced time through the convenience of AWS VPC, and 3) the ability to control the resource placement and environment

What is the purpose of the Amazon VPC?::It is used to establish boundaries around your AWS resources

What is the purpose of the Amazon virtual private gateway?::It is used to allow protected internet traffic to enter into the VPC

What is the purpose of a VPN connection?::It encrypts the internet traffic and helps to protect it from anyone who might try to intercept or monitor it

What is the AWS Client VPN service?::It is a networking service that can be used to connect remote workers and on-premise networks

What features does the AWS Client VPN provide?::It provides a fully elastic VPN service that automatically scales up and down based on user demand where users do not have to install or manage hardware

What is a common use case of the AWS Client VPN?::It can be used to quickly scale remote-worker access

What does Amazon's Site-to-Site VPN do?::It creates a secure connection between your data center or branch offices and your AWS Cloud resources

What are some common use cases of Amazon's Site-to-Site VPN?::It can be used for application migration and secure communication between remote locations

What does AWS PrivateLink do?::It is a highly available, scalable technology that allows you to connect your VPC to services or resources as if they were in your VPC without needing to use an internet gateway, NAT device, public IP address, Direct Connect connection or AWS Site-to-Site VPN connection

What 3 examples of use cases for AWS Direct Connect?::1) it can be used for latency sensitive applications, 2) it can be used to help ensure smooth and reliable data transfer at massive scale for real time analysis, 3) it can be used to connect AWS and on-premise networks to build applications that span without compromising performance

What is the purpose of an AWS Transit Gateway?::An AWS transit gateway is used to connect your Amazon VPC and on-premise networks through a central hub. These AWS Transit Gateways are used to connect inter-Region peering connect transit gateways together

What is the Network Address Translation (NAT) Gateway?::A NAT gateway is a NAT service that can be used such that instances in a private subnet can connect to services outside your VPC but external services can't initiate a connection with those instances

What is an Amazon API Gateway?::The Amazon API Gateway is an AWS service for creating, publishing, maintaining, monitoring, and securing APIs at any scale

What are the 2 general ways to control your network traffic in your VPC with regards to security in the context of AWS?::1) through security groups, or 2) through network access control lists or network ACLs

What is the key difference between security groups and network ACLs with regards to network security?::Security groups are stateful while network ACLs are stateless

What is the default configuration for a custom network ACL in AWS?::The default is that all inbound and outbound traffic is denied until you add rules to specify which traffic to allow

What is the default configuration for a security group in AWS?::The default is to deny all inbound traffic and allow all outbound traffic

What does it mean for a security group within AWS to be stateful?::It means that when a packet response for that request returns to the instance, the security group remembers your previous request and the decision that was made

How does a security group within AWS all for an outbound traffic to receive a response when no inbound traffic is allowed?::It allows inbound traffic response from the outbound packet by remembering the previous request and allowing the response to proceed regardless of inbound security group rules

What is the scope of security group configurations within AWS?::They are scoped at an instance level

What is the scope of a network ACL configuration within AWS?::They are scoped at a subnet level

What types of rules are allowed for security group configurations within AWS?::Only allow type rules are allowed

What types of rules are allowed for network ACL configurations within AWS?::Both allow and deny type rules are allowed

Based on the AWS shared responsibility principle, who owns the responsibility of setting up the AWS security groups and the network ACLs?::The customer using the AWS services is responsible for setting up the AWS security groups and the network ACLs

What does CIDR stand for?::It stands for Classless Inter-Domain Routing

What are 4 examples of the routing policies for routing endpoints?::1) latency-based routing, 2) geolocation, 3) geoproximity, and 4) and weighted round robin

What is Edge Networking?::Edge networking is the process of bringing information storage and computing abilities closer to the devices that produce that information and users who consume it

What is the name of the process of translating a domain name into an IP address?::DNS Resolution

What is the purpose of Amazon Route 53?::Route 53 is a DNS that provides a reliable and cost-effective way to route end users to the internet applications with globally dispersed DNS servers and automatic scaling. Additionally Route 53 can be used to manage DNS records for domain names

What is the purpose of AWS Global Accelerator?::AWS Global Accelerator is a service that uses the AWS global network to improve application availability, performance, and security using intelligent routing and fast failover if something goes wrong in one of the application locations

When might it be useful to use both VPN and AWS Direct Connect?: A common use case for using both at the same time is to use AWS Direct Connect as the main connection and use VPN as a failover

## Module 6 - Storage
What are the 3 types of data options that can be stored in AWS?::Block, object and file storage

What does Block storage in AWS store?::It stores data into manageable pieces called blocks

How can data in Block storage be updated in AWS?::It can be updated block by block, which means that the entire file doesn't need to be changed every time you make an update

What is an ideal use case for Block storage within AWS?::Block storage in AWS can be used for applications or databases that need quick and frequent updates

What does Object storage store in AWS store?::It saves data in self-contained units as objects

What must each object within Object storage in AWS contain?::Each must contain data, a unique ID, and metadata about the object that makes it easy to organize and retrieve

How can data in an Object storage be updated in AWS?::Updates to objects in the object storage requires updating of the entire object for every change

How are Objects organized in AWS store?::Objects are organized in flat structures called buckets

What is an ideal use case for Object storage within AWS?::It is ideal for files that don't change constantly like videos, backups, or logs

How is File storage in AWS organized?::File storage is organized in hierarchical file systems that can be shared by applications and are compatible with most systems with little to no code modification in most cases

What is an ideal use case for File storage within AWS?::File storage is ideal for applications which require shared access, like content management systems

What are the 2 AWS Block storage options provided?::The AWS EC2 instance store and Amazon Elastic Block Store (EBS)

What does AWS EC2 instance store provide?::It provides unmanaged non-persistent, high performance block storage directly attached to EC2 instances for temporary data

What does AWS Elastic Block Store provide?::It provides a managed service that provides block storage volumes for EC2 instances, offering various types for different workloads

What is the name of the AWS storage option that provides Object storage?::Amazon Simple Storage Service (S3)

What does the Amazon Simple Storage Service provide?::It provides fully managed scalable object storage service for storing and retrieving any amount of data from anywhere

What are the 2 AWS File storage options provided?::The Amazon Elastic File System (EFS) and Amazon FSx

What does Amazon Elastic File System provide?::It provides a fully managed, scalable NFS file system for use with AWS Cloud Services and on-premise resources

What does Amazon FSx provide?::It provides a fully managed file storage service for popular file systems like Windows, Lustre, and NetApp ONTAP

Between Amazon FSx and Amazon Elastic File System, which storage solution should be used when wanting to support Linux-based applications with standard file interfaces?::Amazon Elastic File System should be used as Amazon FSx is more tailored for Windows, Lustre and NetApp ONTAP use cases

What does Amazon Storage Gateway provide?::It provides a fully managed, hybrid-cloud storage service that provides on-premises access to virtually unlimited cloud storage

What does Amazon Elastic Disaster Recovery provide?::It provides a fully managed service that streamlines the recovery of your physical, virtual, and cloud-based servers into AWS

What are the 3 categories of the AWS shared responsibility model for services?::1) fully managed services, 2) managed services, and 3) unmanaged services

What are the 4 things that fall under AWS's responsibility for managed storage services?::AWS is responsible for 1) platform and application management, 2) OS, network and firewall configuration, 3) software for compute, storage, database and networking, 4) hardware and AWS global infrastructure.

What are the 2 things that fall under AWS's responsibility for unmanaged storage service?::AWS is responsible for 1) the software for compute, storage, database and networking and 2) the hardware, and AWS global infrastructure

What is an instance storage volume with regards to AWS block storage?::It is a physically attached local storage

What happens to the contents written to an instance storage volume when the EC2 instance is stopped?::All of the data written to the instance store volume will be deleted

What happens to the contents written to an EBS volume when the EC2 instance is stopped?::The data will be persisted between instances of starting and stopping an EC2

What is the benefit to instance storage volumes in EC2?::The instance store volume comes automatically attached to many EC2 instance types and provides temporary block-level storage at no additional cost and since it is physically connected, it offers high I/O performance for data that disappears when the instance stops

What are the 2 configuration parameters that must be provided when creating an EBS volume?::1) the volume size, 2) the type

What is an EBS snapshot?::An EBS snapshot is a back up of the data within an EBS volume

What are 3 usecases of EBS snapshots?::It can be used for database hosting, backup storage for applications, and rapid deployment of development environments using volume snapshots

What are 5 benefits of EBS volumes?::1) it can be easily migrated across Availability Zones, 2) it can be used by instances of different types, 3) the EBS snapshots provide reliable backup solutions that can be restored in different regions, 4) EBS volumes can be modified to different types and sizes to match actual usage patterns without downtime, 5) AWS EBS provides different volume types to match different workload requirements

What are the 4 high-level tasks that are involved in managing your EBS volume lifecycle?::1) provisioning, 2) moving, 3) deprovisioning, and 4) backing up of the volume

What does it mean to have an incremental backup?::At each backup, the first time data is stored, it contains all the data and subsequent backups only contain the data that has changed since the previous snapshot

What does the Amazon Data Lifecycle Manager provide?::It provides a managed service that can define policies to help automate the snapshot lifecycle management

What are the 4 things that you can do with Amazon Data Lifecycle Manager?::1) You can schedule snapshot creation, 2) set retention policies, 3) manage lifecycles, and 4) apply consistent backup policies

What are the 3 benefits of EBS Snapshots?::1) It enables Data protection and recovery, 2) it provides operational flexibility as it can be used for cross-Region migration, volume resizing, volume cloning, and data sharing across AWS accounts, 3) it is cost effective as incremental backups only store the information that has changed

With regards to S3 storage, what kind of S3 datatype is used to store single data files?::Single data files are stored as objects

What is the maximum size of an individual object within AWS S3 storage?::5 terabytes

What is the maximum on the total bucket size?::There is no maximum on the total bucket size

How can you protect objects in AWS S3 storage from deletion?::They can be protected from deletion by turning on versioning for the objects in S3

What are 4 common use cases for S3 storage?::1) hosting websites, 2) storing backups, 3) archiving data, and 4) managing multi-media like videos or images

What are the 3 components of an object within S3 storage?::1) the data itself, 2) the metadata, 3) a unique identifier

At what scale of uniqueness is the name of a bucket in S3 storage?::A bucket name is unique globally

What are the 3 advantages of using AWS S3?::1) AWS buckets have virtually unlimited storage, 2) the S3 lifecycle policies automatically move objects between storage classes based on user defined rules to optimize cost over time, 3) S3 supports a wide range of use cases for both cloud-based applications and traditional on-premise applications

What does AWS's S3 storage's bucket policy allow you to control?::It allows you to specify which actions are allows or denied on the bucket in addition to every object in that bucket

What is AWS's S3 identity-based policy?::It provides permissions that control what actions users, groups, or roles can perform on S3 resources by identities instead of by resource

What are the 2 encryption offerings provided by AWS's S3?::1) Encryption at rest and 2) encryption in transit

What level of durability does Amazon S3 storage provide to users for objects stored across availability zones?::99.99999999999 (11 9s)

What does the storage class "Standard-IA" mean with respect to S3 storage and is used for?::It stands for infrequent access and is a more cost-effective solution

What is a good use case for "Standard-IA" storage in S3 storage?::It is a good for backups

What does the storage class "Glacier" mean with respect to S3 storage and is used for?::It is meant for long-term archiving

What is the purpose of the S3 Glacier Instant Retrieval with respect to S3?::It is meant for quick access of data in occasion but still want to optimize for cost as much as possible

What are 2 good example use case for S3 Glacier Instant Retrieval?::1) Medical image or 2) media file storage

What is the purpose of Glacier Flexible Retrieval with respect to S3?::It is meant for long-term archiving where the user can tolerate slower data retrieval

What is the storage class "Glacier Deep Archive" mean with respect to S3 storage and is used for?::It is used for storing data you hardly ever need and is the most cost-effective option

What are 2 examples of use cases for S3 Glacier Deep Archive?::1) Compliance archives and 2) digital preservation

How quickly can data in S3 Glacier Deep Archive be restored?::It can be restored in 12 hours

What are the 2 one-zone specific storage tiers in AWS S3 storage?::1) S3 Express One Zone and 2) S3 Zone-IA

What is the downside of using one-zone specific storage tiers in AWS S3?::The data is susceptible to data loss in the unlikely case of the loss or damage to all or part of an AWS Availability Zone

What are the 9 storage classes in AWS S3 storage?::1) Standard, 2) Intelligent-Tiering, 3) Standard Infrequent Access, 4) One Zone Infrequent Access, 5) Express One Zone, 6) Glacier Instant Retrieval, 7) Glacier Flexible Retrieval, 8) Glacier Deep Archive, 9) S3 Outposts

What does S3 Lifecycle configurations allow you to do?::It allows you to automate the process of managing object storage tier configurations

What are the 2 types of actions that can be used to automate an object or group life cycle in S3 storage?::1) Transition actions, 2) Expiration actions

What does a transition action with regards to AWS S3 storage define?::It defines when objects should transition to another storage class

What does an expiration action with regards to AWS S3 storage define?::It defines when objects expire and should be permanently deleted

What are 2 candidate use cases for using with AWS S3 storage lifecycles?::It can be used with 1) periodic lots that your application might need for a week or a month where after that they would be deleted, or 2) data that changes in access frequency for example where it might be frequently accessed for a limited period of time, but at some point real-time access is no longer required

What does EFS stand for in relation to AWS's storage solutions?::It stands for Elastic File System

What are 3 benefits of EFS over EBS?::1) Multiple instances can read and write to the same EFS at a time where as EBS is most closely like a storage volume that is attached to an EC2 instance, 2) EFS is not AZ limited and 3) EFS can automatically scale as the space fills up 

How is storage life cycle managed within Amazon EFS?::Amazon EFS manages data between storage classes based on access patterns automatically while providing the customer with the option to customize the lifecycle policies to fit their storage needs

What protocol is used with Amazon EFS?::Amazon EFS uses the Linux NFS protocol

What are the 5 storage classes in Amazon EFS?::1) Standard storage class, 2) Standard IA, 3) One Zone, 4) One Zone IA, 5) Archive

What does Amazon EFS Standard and Standard IA provide as a storage class?::It provides multi-AZ resilience and the highest levels of durability and availability

What does Amazon EFS One Zone and Amazon EFS One Zone IA provide as a storage class?::It provides additional savings by saving data in a single availability zone 

What does Amazon EFS Archive provide as a storage class?::It provides a cost-optimized solution for data that is accessed only a few times a year or less and that does not need the sub-millisecond latencies but provides up to 50% lower compared to EFS IA

What does the policy Transition to IA in Amazon EFS do?::It automatically transitions files into IA if it has not been accessed in Standard storage for 30 days

What does the policy Transition to Archive in Amazon EFS do?::It automatically moves files into Archive from Standard if it has not been accessed for 90 days

What does the policy Transition to Standard in Amazon EFS do?::It automatically transitions files out of IA or Archive and back into standard storage when files that are in IA or archive have been accessed

What does Amazon FSx provide?::It makes it convenient and cost effective to launch, run and scale feature-rich, high-performance file systems in the cloud

What are the 4 file system protocol supported by Amazon FSx?::1) Windows File Server, 2) Lustre, 3) OpenZFS, 4) NetApp ONTAP

What are the 4 benefits of using Amazon FSx?::1) Filesystem integration, 2) managed infrastructure, 3) scalable storage, 4) and it is cost effective

What are 4 example use cases of using Amazon FSx for Windows File Server?::To 1) Migrate Windows file servers to AWS, 2) Accelerate hybrid workloads, 3) reduce SQL server deployment costs, and 4) to streamline virtual desktop and streaming

What are 4 example use cases of using Amazon FSx for NetApp ONTAP?::To 1) Migrate workloads to AWS seamlessly, 2) build modern applications, 3) modernize your data management, and to 4) streamline business continuity

What are 4 example use cases of using Amazon FSx for OpenZFS?::To 1) Migrate workloads to AWS seamlessly, 2) Deliver insights faster for data analytics workloads, 3) Accelerate content management, 4) increase dev/test velocity

What are 4 example use cases of using Amazon FSx for Lustre?::To 1) Accelerate Machine Learning, 2) enagble high performance computing, 3) unlock big data analytics, 4) increase media worklaod agility

What does is AWS Storage Gateway?::It is a hybrid cloud storage service that is like a bridge that provides virtually unlimited cloud storage

What are the 3 types that AWS Storage Gateway comes in?::It comes in 1) S3 File Gateway, 2) Volume Gateway, 3) Tape Gateway

What are the 4 benefits of using AWS Storage Gateway?::1) It provides seamless integration between on-premise applications and AWS cloud storage, 2) it provides a centralized management of hybrid storage environments, 3) it has local caching for frequently accessed data, and 4) it reduces on-premise storage costs by using cloud storage for data archiving, backup and disaster recovery purposes

What does the Amazon S3 File Gateway category within AWS Storage Gateway provide?::It bridges a user's local environment with Amazon S3 storage and provides on-premise applications with unlimited cloud storage through familiar file protocols

How does a user interact with Amazon S3 File Gateway?::It appears to your local systems as a standard file server where files written to this server are automatically uploaded to Amazon S3 while maintaining local access to recently used data through intelligent caching

What does the Amazon S3 Volume Gateway category within AWS Storage Gateway provide?::It creates a bridge between a user's on-premise infrastructure and AWS Cloud storage by presenting the user's cloud data as iSCSI volumes that can be mounted by existing applications

What are the 2 configurations of volume gateways?::In 1) cached volume mode and 2) in stored volume mode

What does the Amazon S3 Tape Gateway category within AWS Storage Gateway provide?::It provides a way to replace existing physical tape infrastructure within virtual tape capabilities by providing an interface that works with existing tape backup software

How does Amazon S3 Tape Gateway show up to backup applications?::It shows up as standard tape hardware to backup applications

What does Amazon Elastic Disaster Recovery program provide?::It provides block-level data replication to AWS in both physical and virtual servers to enable rapid recovery during disruptions

How does Amazon Elastic Disaster Recovery program allow for cost optimization?::It eliminates expensive secondary data centers and only pay for what you use, with minimal upfront investment and no standby infrastructure cost

How does Amazon Elastic Disaster Recovery program provide business resilience?::It provides this by providing continuous block-level data replication and the ability to recover workloads within minutes during disruptions

How does Amazon Elastic Disaster Recovery program provide a streamlined process for disaster recovery?::An intuitive console that reduces manual configurations allows for the automation of disaster recovery

What are 3 use cases of Amazon Elastic Disaster Recovery?::1) Hospital data protection, 2) Financial services continuity, and 3) manufacturing operations recovery

In the case where there are rapid read and write operations being performed, should Amazon EBS or Amazon S3 be used?::EBS provides block level access which works well for databases which more easily supports rapid read and write operations

Why would it not be appropriate to use Amazon Volume Gateway in Cached mode for storing a collection of files both on-premises as well as in cloud storage for cost savings?::While using Amazon Volume Gateway in Cached mode keeps frequently accessed data local, it presents cloud storage as iSCSI block storage volumes instead of file shares which is more preferred by the user

## Module 7 - Databases
What are the 3 categories of groupings of database services in terms of ownership with respect to the AWS's shared responsibility principle?::Fully managed services, managed services and unmanaged services

What are the 6 things that Amazon is responsible with fully managed services with regards to AWS's databases?::Amazon is responsible for 1) server side encryption, 2) network traffic protection, 3) platform and application management, 4) OS, network and firewall configuration, 5) the software for compute, storage, database and networking, and 6) the hardware and AWS global infrastructure

What are the 4 things that Amazon is responsible for with managed services with regards to AWS's databases?::Amazon is responsible for 1) platform and application management, 2) OS, network and firewall configuration, 3) software for the compute, storage, database and networking, and 4) the hardware, and AWS global infrastructure

What are the 2 things that Amazon is responsible for with unmanaged services?::Amazon is responsible for 1) the software for compute, storage, database and networking and 2) the hardware, and AWS global infrastructure

What does Amazon RDS stand for?::It stands for Amazon Relational Database Service

What does Amazon RDS provide?::It provides a managed relational database service that handles routine database tasks like backups, patching, and hardware provisioning

What kinds of backup services does Amazon RDS provide?::It offers multi-AZ deployment, automated backups and also manual backups using DB snapshots

What are the 3 security features provided by Amazon RDS?::1) Network isolation, 2) encryption in transit, 3) encryption at rest

What are the 6 relational database engines provided by Amazon RDS?::1) Amazon Aurora, 2) MySQL, 3) PostgreSQL, 4) Microsoft SQL Server, 5) MariaDB, and 6) Oracle DB

How does Amazon RDS provide the benefit of cost optimization?::It eliminates the high upfront cost of purchasing and maintaining database hardware infrastructure and users only pay for compute and storage resources that are being consumed. Operational costs related to performing administrative backup tasks are also automated

What are 3 ways that Amazon RDS provide the benefit of performance optimization?::1) It provides automated management of resource allocation, 2) it has insights regarding performance monitoring, and 3) it creates read replicas that help offload read traffic from the primary instance

What is Amazon Aurora?::It is a managed relational database service

What are the 3 benefits provided by Amazon Aurora?::1) It is compatible with MySQL and PostgreSQL with 5 and 3 times the throughput respectively, 2) it has automated scaling with continuous backups to Amazon S3 storage, 3) it replicates across 3 availability zones with 6 copies of the data, providing 99.99% availability

What does Amazon Dynamo provide?::It provides a fully managed NoSQL database service with fast and predictable performance for both document and key-value data structures

How does Amazon Dynamo structure and store data?::The data is structured and stored in key-value pairs

What does the Amazon Dynamo feature "DynamoDB global tables" allow?::It allows Amazon DynamoDB to be used for globally distributed applications

What are 3 example use cases of using Amazon DynamoDB?::It can be used for 1) gaming platforms, 2) financial service applications, and 3) mobile applications with global user bases

In what way does Amazon DynamoDB provide the feature of Consistent high performance?::It delivers single-digit millisecond response times at any scale

What level of data availability and durability does Amazon DynamoDB provide?::It provides 99.999% data availability

How does Amazon DynamoDB provide high data availability and durability?::It replicates data across 3 distinct facilities within each AWS Region and maintains multiple copies in separate AWS Region

What kind of data encryption does Amazon DynamoDB provide?::It provides encryption at rest and in transit while providing the flexibility to choose between different kinds of encryption keys for customized security control

What does Amazon's in-memory caching services provide?::It provides sub-millisecond latency for read and write operations

What are 4 examples of use cases that work well with the low latency provided by in-memory caching services?::It is useful for 1) storing session data, 2) caching API responses, 3) data base query results, and other information that applications require repeatedly

What does Amazon's ElastiCache provide?::It provides a fully managed in-memory caching service that is built to reduce the complexity of administering in-memory caching systems

In what ways does Amazon's ElastiCache provide high-performance?::It provides high-availability offerings for services like Redis, Valkey, Memcachedand and automatically handles hardware provisioning, software patching, and monitoring

How does Amazon's ElastiCache provide high-availability?::It does so by constantly monitoring primary nodes for potential failures and when an issue is detected, it promotes a replica node to become the new primary node without manual intervention. It also completes typical recovery processes within minutes to minimize downtime

What does Amazon DocumentDB provide?::It provides a fully managed service for hosting databases whose data does not conform to rigid relational schemas

What is kinds of applications is Amazon's DocumentDB most suited for?::It is suited for applications requiring frequent schema changes and document-oriented data

What are 3 example use cases that work well for Amazon's DocumentDB service?::Amazon's DocumentDB works well for 1) content management systems, 2) catalog and inventory management, and 3) user profile and personalization systems

What system is Amazon's DocumentDB compatible with?::It is compatible with MongoDB

How does Amazon's DocumentDB provide increased read throughput?::It does so by providing up to 15 replica instances that share underlying storage

What does the AWS Backup service provide?::It provides streamed data protected across various AWS resources and on-premise deployments by providing a single dashboard for monitoring and managing backups

What are the 3 storage types supported by AWS Backup?::1) Amazon EBS, Amazon EFS and various databases

What are 3 example use cases of AWS Backup?::It can be used for 1) centralized disaster recovery, 2) consistent backup policies for compliance requirements, and 3) consolidating multiple back up processes through a single interface

What does Amazon Neptune provide?::It is a fully managed service with purpose-built graph database service for highly connected data sets like those used in social networking applications

What are 3 example use cases of Amazon Neptune?::It can be used for 1) social network user connection mapping, 2) fraud detection systems, and 3) search and recommendation systems

What problem does AWS Database Migration Service solve?::It minimizes application downtime during database migrations

How does Amazon Aurora compare against other high-end relational database offerings?::It provides comparable performance but at a tenth of the cost

## Module 8 - AI ML and Data Analytics
What does Amazon Polly service provide?::It provides text-to-speech generation

What does Amazon Comprehend provide?::It provides text or sentiment analysis

What does Amazon SageMaker AI provide?::It provides a way to build, train, and deploy your own ML models using fully managed infrastructure, tools and workflows

What are 3 common ML model use cases?::1) Prediction of trends, 2) making decisions, and 3) detection of anomalies

What are the 3 tiers of solutions that AWS's AI/ML stack comprises of?::1) AI services, 2) ML services, and 3) ML frameworks and infrastructure

What do prebuilt AI services in AWS provide?::They provide pre-built models that are already trained to perform specific functions

What are the 3 groups of AWS AI services provided?::1) Language services, 2) computer vision and search services, and 3) conversational AI and personalization services

What are the 4 services provided by AWS's AI language services?::1) Amazon Comprehend, 2) Amazon Polly, 3) Amazon Transcribe, and 4) Amazon Translate

What are the 3 services provided by AWS's AI computer vision and search services?::1) Amazon Kendra, 2) Amazon Rekognition, 3) Amazon Textract

What does Amazon Kendra provide?::It provides a service that uses natural language processing to search for answers within large amounts of enterprise content

What are some 3 example use case of Amazon Kendra?::1) Intelligent search, 2) chatbots, and 3) application search integration

What does Amazon Rekognition provide?::It provides video analysis as a service to identify objects, people, text, scenes, and activities within images and videos stored in Amazon S3

What are 4 example use cases of Amazon Rekognition?::1) Content moderation, 2) identity verification, 3) media analysis, and 4) home automation

What does Amazon Textract provide?::It provides a service that can detect and extract typed and handwritten text found in documents, forms and even tables within documents

What are 3 example use cases of Amazon Textract?::It can be used for 1) financial, 2) healthcare, and 3) government form text extraction for quick processing

What does Amazon Lex provide?::It provides voice and text conversational interfaces to your applications

What mechanisms are used by Amazon Lex?::Amazon Lex uses natural language understanding (NLU) and automatic speech recognition (ASR)

What are 3 example use cases of Amazon Lex?::It can be used for 1) virtual assistants, 2) natural language search for FAQs, and 3) automated application bots

What does Amazon Personalize provide?::It provides a service where given historical data, it can be used to build intelligent applications with personalized recommendations for your customers

What are 3 example use cases for Amazon Personalize?::1) Personalized streaming, 2) product, and 3) trending recommendations

What does Amazon SageMaker AI provide?::It provides a fully managed service where you can build, train, and deploy your own ML models without worrying about infrastructure

What are the 3 benefits of Amazon SageMaker AI?::1) It provides a wide range of tool choices with a no-code interface, 2) it provides a high-performance, cost-effective infrastructure, and 3) it allows you to automate and standardize your MLOps practices and governance across your enterprise for transparency and auditabililty

What does SageMaker Jumpstart provide?::It is a machine learning hub with foundational models and prebuilt ML solutions that can be deployed with a few clicks

What does Amazon Bedrock provide?::It is a fully managed service that offers a broad choice of high-performance, pre-trained models from Amazon and other leading AI companies

What does Amazon Q provide?::It provides an interactive assistant that can be tailored to your business where it integrates seamlessly with your company's information repositories so it can engage in contextualized conversations, provide insightful solutions, and complete actions relevant to your organization

What are foundational models?::Foundational models are extremely large models that are pre-trained on vast collections of data

What are 3 examples of common use cases provided by Amazon SageMaker JumpStart?::1) Rapid ML model deployments without extensive ML expertise, 2) custom file-tuned solutions to a user's specific domain, and 3) for performing performance comparison between different models before committing to a specific approach

What are 3 example of common use cases provided by Amazon Bedrock?::1) Building of enterprise-grade generative AI, 2) for creating multi-modal content generation, and 3) for creating advanced conversational agents that connect to enterprise data to provide accurate responses

What are the 2 products available with Amazon Q?::1) Amazon Q Business and 2) Amazon Q Developer

What does Amazon Q Business do?::It can help answer pressing questions, help solve problems, and take actions using data and expertise found in your company's information repositories

What does Amazon Q Developer do?::It can provide code recommendations to accelerate development for coding languages including C#, Java, Javascript, Python, and TypeScript applications

What are the 3 steps involved with the process ETL in data analytics?::1) Extraction of data from various source systems, 2) then transform the data into a consistent usable format, and 3) load the data into a destination system

What are the 3 steps involved with the process ELT in data analytics?::1) Extraction of data from various source systems, 2) load data into the tools and 3) transform the data as needed

What does Amazon Kinesis Data Streams provide?::It is a serverless service that can be used for real-time ingestion of terabytes of data from applications, streams, and sensors

What does Amazon Data Firehose provide?::It is a fully managed service that provides near real-time data ingestion and also delivery of data to data lakes, warehouses, and analytics services within seconds

What does Amazon Redshift provide?::It provides a fully managed data warehouse service that can store petabytes of structured or semistructured data with the scalability of pay-as-you-go pricing model

What does Amazon Glue Data Catalog provide?::It provides a centralized, scalable, and managed metadata repository that enhances data discovery

What does Amazon Glue provide?::It provides a fully managed ETL service that makes data preparation simpler, faster, and cost effective.

What does Amazon EMR provide?::It provides data processing with popular frameworks like Apache Spark, Apache Hadoop, and Apache Hive while automatically handling infrastructure provisioning, cluster management, and scaling

What does Amazon Athena provide?::It provides a fully managed service that can access data hosted on Amazon S3, on premises or even in multi-cloud environments through SQL queries in relational, non-relational, objects and custom data sources

What does Amazon QuickSight provide?::It provides a platform where technical and non-technical users can quickly create modern interactive dashboards and reports from various data sources without managing infrastructure

What does Amazon OpenSearch Service provide?::It provides a platform to perform search for relevant content through precise keyword matching or natural language queries

What AWS 2 services are used for data ingestion?::1) Amazon Kinesis Data Streams and 2) Amazon Data Firehose

Between structured, semi-structured and un-structured, what types of data is Amazon Redshift designed to store?::It is designed to store structured or semi-structured data

## Module 9 - Security
What does the authentication process do?::Authentication is the process of verifying the identity of the user or entity

What does the authorization process do?::Authorization determines which actions a user is permitted to perform on a system or application

What are the 4 things that a customer is responsible for securing in the AWS cloud?::1) the security of data, systems and applications, 2) deciding what data and workloads to store or run in AWS, 3) determining which AWS services to use, and 4) controlling who has access to environments and resources

What are some examples of 3 things that AWS's security mechanisms allow help you do?::1) Prevent security incidents through proper permissions and access management, 2) protect networks, applications and data, and 3) detect and respond to security incidents as they occur

What kinds of access does an AWS root user have?::The root user is the owner of the account and has permissions to do anything they want inside of that account

What does IAM stand for with regards to AWS security?::It stands for Identity and Access Management

What are the default permissions for IAM?::By default, all actions are denied in IAM

What is the principle of least privilege?::The principle of least privilege dictates that you should only give people and systems access to what they need and nothing else

What is an IAM user account used for?::It is to represent a person or application that interacts with AWS services and resources. 

What is an IAM group?::It is a collection of IAM users

What happens to the access of all IAM users within a IAM group when a permissions is applied to a group?::All users within the group will inherit the permissions

What is an IAM role?::An IAM role is an identity you can assume to gain temporary access to permissions

What is an IAM policy?::An IAM policy is a JSON document that allows or denies permissions to access AWS services or resources

What is the purpose of AWS IAM Identity Center?::The IAM Identity Center centralizes identity and access management across AWS accounts and applications

What is Federated Identity Management?::Federated Identity Management is a system that allows users to access multiple applications, services, or domains using a single set of credentials

What is the purpose of AWS Secrets Manager?::The AWS Secrets Manager provides a secure way to manage, rotate, and retrieve database credentials, API keys, and other secrets throughout their lifecycle

What is the definition of a secret with regards to security?::Secrets are confidential or private information intended to be known only to specific individuals or groups

What is the purpose of the AWS Systems Manager?::The AWS Systems Manager provides a centralized view of all nodes across our organization's accounts and Regions and multi-cloud and hybrid environments

What is a Node in AWS with regards to AWS System Management?::A Node is a connection point in a network, system or structure

How does a Security group help solve the problem of DDoS attacks in AWS?::Security groups only allow in proper request traffic. If traffic does not match the expected customer protocol, the content is not allowed to communicate with the server

What level does Security groups operate at and how does that help handle DDoS attacks on AWS EC2 instances?::The Security groups operate at the AWS network level which allows it to handle massive UDP attacks due to the capacity of an entire AWS Region.

What is a way to use managed services provided by AWS to handle DDoS attacks?::Instead of making an EC2 instance a gateway, use an ELB the gate way

What does AWS Shield Standard provide?::It provides protection to AWS resources from most common, frequently occurring types of DDoS attacks

What are the 3 managed services that provide AWS Shield Standard at no extra cost?::1) Elastic Load Balancer, 2) CloudFront, and 3) Route 53

What does AWS Web Application Firewall do?::It filters incoming traffic for signatures of bad actors

What does AWS Shield Advanced provide?::It provides detailed attack diagnostics and the ability to detect and mitigate sophisticated DDoS attacks

What does AWS Security Groups do?::AWS Security Groups only allow in proper request traffic and operate at the AWS network level

What mechanism does AWS WAF use to control traffic?::It controls traffic by checking incoming IP addresses against a web access control list and if the IP is on the blocked list, it denies access

How does Amazon S3 provide built in security protection?::By default all S3 buckets have encryption configured, and all uploaded objects are encrypted at rest

How does Amazon EBS provide built in security protection?::Amazon EBS volumes can be encrypted at rest, including both boot and data volumes of an EC2 instance

How does Amazon DynamoDB provide built in security protection?::Server-side encryption is enabled on all DynamoDB table data using encryption keys stored in AWS Key Management Service

What does Amazon Key Management Service provide?::It is a service that can be used to create and manage cryptographic keys.

What does Amazon Macie provide?::Amazon Macie allows you to monitor your sensitive data at rest to make sure it is safe.

How does Amazon Macie help to monitor sensitive data?::It uses machine learning and automation to discover sensitive data stored in Amazon S3 and allows you to adjust your security posture

What does AWS certificate manager provide?::It centralizes the management of SSL/TLS certificates that provide data encryption in transit

What are SSL/TLS certificates used for?::They are used to establish encrypted network connections from one system to another

What is the purpose of Amazon Inspector??Amazon Inspector helps to bring attention to potential vulnerabilities

How is Amazon Inspector used?::It can be used to run automated security assessments against your infrastructure and it will check on deviations of security best practices, exposure of EC2 instances and vulnerable version installs

What is the purpose of Amazon GuardDuty?::It is a service that analyzes continuous streams of your account metadata and network activity as it looks for threats

What is the purpose of Amazon Detective?::Amazon Detective is used to uncover the root cause of security issues after a threat has been detected

What are the 3 actions performed by Amazon Detection?::1) It automatically collects log data from your AWS resources and uses machine learning and graph analytics to build interactive visualizations of detected issues

What is the purpose of AWS Security Hub?::AWS Security Hub is used to bring multiple services together into a single place and format. This service aggregates security findings from AWS and partner services and organizes them into actionable, meaningful groupings called insights

## Module 10 - Monitoring, Compliance and Governance in AWS Cloud

What are the 4 things that would be needed in general to perform effective monitoring of Amazon Web Services?::One would need ways to provide insight into 1) resource utilization, 2) ability to identify potential issues, and 3) ability to facilitate proactive problem resolution

What are the 4 steps in the progression of monitoring resources?::1) Securing systems, 2) monitoring activities, 3) conducting audits, and 4) ensuring compliance

What does it mean to Secure resources within AWS?::It means to protect data, systems, and infrastructure from unauthorized access, use, disclosure, disruption, modification, or destruction

What does it mean to Monitor resources within AWS?::It means to continuously observe analyze system activity, network traffic, and security events to detect potential threats or anomalies

What does it mean to Audit resources within AWS?::It means to periodically review and assess the effectiveness of security controls and check that all requirements are met and security policies and procedures are adhered to

What does it mean to have Compliance within AWS?::It means to help ensure that an organization's security practices and controls meet the requirements of relevant regulations, industry standards, and contractual obligations

What does Amazon CloudWatch allow you to do?::It allows you to monitor your infrastructure and applications that run on AWS through the tracking of metrics

What does the feature CloudWatch alarms allow you to do with Amazon CloudWatch?::It allows you to create an alarm then set a threshold for a metric and when the threshold is reached, an alarm is triggered and we can add a corresponding action

What does the feature CloudWatch metrics allow you to do with Amazon CloudWatch?::Amazon CloudWatch metrics would collect metrics from all of your AWS resources, applications and services within AWS and on-premises server

What does the feature CloudWatch dashboards allow you to do with Amazon CloudWatch?::It allows you to have a customizable home page to monitor your resources in a single view

What does the feature CloudWatch logs allow you to do with Amazon CloudWatch?::It centralizes the logs from all of your systems, applications and AWS services that is used

What does AWS CloudTrail provide?::It provides a log of every user activity and API usage that was made in AWS for auditing purposes

How long does AWS CloudTrail keep a record of user actions, API usages and other transactions?::The record is kept for 90 days

Where are the logs of events recorded by AWS CloudTrail stored?::They are stored in Amazon S3

What are examples of 2 industries that can use the logged events from AWS CloudTrail for their audit due to the secure storage of these logs?::Payment Card Industry (PCI) and Healthcare Insurance Portability and Accountability Act (HIPAA)

What does AWS CloudTrail insights do when enabled?::It will analyze the normal patterns of API call volume and API error rates to generate insights events when the call volumes and error rates deviate from the normal patterns

What is the purpose of AWS Config?::It is a service that monitors and records AWS resource configurations and evaluates them against the configurations you want to implement to help you assess, audit and evaluate your resources

What is the purpose of AWS Audit Manager?::It is a fully managed service that automates the evidence collection to reduce the manual effort of several cross-functional teams that audit activities often require

What is an example use cases of AWS Config?::To continually audit security monitoring and analysis to streamline operational troubleshooting and change management

What is the purpose of AWS Organizations?::It is an account management service that allows for the consolidation and central management of multiple AWS accounts within an organization

What are the 5 things that AWS Organizations can be used to manage for the AWS accounts within an organization?::The 1) billing, 2) access, 3) compliance, 4) security and 5) shared resources allocation across accounts

What kind of hierarchy exists between accounts in AWS Organizations?::There can be a main or parent account where all other accounts are secondary or children accounts to that main account. 

With regards to billing within AWS Organization, how is billing aggregated?::The bills for all children accounts are rolled up to the main/parent account

What does the acronym OU stand for with regards to AWS Organization?::It stands for organizational unit

What is the purpose of a OU within AWS Organizations?::The purpose of OUs is to allow the administrator to group all accounts that share the same access or regulatory requirement into a group where access can be controlled

What does SCP stand for with regards to AWS Organizations?::It stands for Service Control Policies

What does SCPs with regards to AWS Organizations specify?::It specifies the maximum permissions for member accounts in organizations

What are the 3 things that SCPs are able to control in terms of access?::It can control the 1) services, 2) resources, and 3) individual API actions

What are the 2 entities that SCPs with regards to AWS Organizations can be applied to?::1) An individual member account and 2) an organizational unit

What does Governance as a framework consist of?::It consists of the work that makes it such that building and operating your deployment is efficient and supports the overall goals

What is the purpose of AWS Control Tower?::It is a service that you can use to enforce and manage governance rules for security, operations, and compliance at scale across your organizations and accounts in the AWS Cloud

What does the AWS Control Tower dashboard allow you to view?::It allows you to view provisioned accounts across your enterprise and also has controls for policy enforcement and help detect non-compliant resources

What does the AWS Control Tower Account Factory provide?::It provides a way to have configurable account templates that standardizes the provisioning of new accounts

What does AWS Control Tower Controls provide?::It provides high-level rules that provide governance for your overall AWS environment

What does AWS Landing Zone provide?::It is a well-architected account environment that's based on security and compliance best practices. 

What does AWS Landing Zone's enterprise container contain?::It contains all of the organizational units, accounts, users, and resources to be regulated

What does the AWS Service Catalog provide?::It allows the user to create, share and organize from a curated catalog of AWS resources such that the user can quickly deploy baseline networking resources and security tools for new AWS accounts to be governed consistently

What are 3 example use cases of AWS Service Catalog?::It can be used to 1) provision resources across AWS accounts, 2) apply access controls, and 3) accelerate provisioning of continuous integration and continuous delivery (CI/CD) pipelines

What does Amazon BYOL stand for?::It stands for Amazon Bring Your Own License

What does Amazon BYOL allow you to do?::It allows you to use existing software licenses purchased directly from vendors such as Microsoft, on AWS services like Amazon EC2 Dedicated Hosts

What is the benefit of Amazon BYOL?::Purchasing and using the license directly from the vendor allows for significant cost savings compared to purchasing the licenses through Amazon

What does Amazon License Manager provide?::It provides a service that helps you manage your software licenses and fine-tine your licensing costs

What are 3 of the example use cases of Amazon License Manager?::1) It is used to streamline license management, 2) to simplify the Microsoft License mobility through Software Assurance experience, and 3) it can be used to automate the distribution and activation of software entitlements across AWS for end users

What is the purpose of AWS health?::AWS Health is the go-to data source for events and changes affecting the health of your AWS Cloud resources

What does the AWS Health Dashboard provide?::It provides valuable information as a data source for events, changes and also gives you timely and actionable guidance to remedy issues

What is the purpose of AWS Trusted Advisor?::It is a service that you can use in your AWS account that will evaluate your resources against 5 categories of checks

What are the 5 categories of checks that are performed with AWS Trusted Advisor?::1) Cost optimization, 2) performance, 3) security, 4) fault tolerance, and 5) service limits

What are some examples of alerts that can be shown for Cost Optimization within AWS Trusted Advisor?::It could check if there are any RDS instances that are idle in this account or it might evaluate the utilization of ELB or EBS

What are some examples of alerts that can be shown for Performance within AWS Trusted Advisor?::It could check for the EBS volumes whose performance might be affected by the throughput capability of the EC2 instance that it's attached to

What are some examples of alerts that can be shown for Security within AWS Trusted Advisor?::It could check for security groups that are allowing public access to EC2 instances which is putting those resources at risk

What are some examples of alerts that can be shown for Fault Tolerance within AWS Trusted Advisor?::It could check for an EBS volume without snapshots in this account where without the snapshot, a failure of that EBS volume would result in a loss of data

What are some examples of alerts that can be shown for Service Limits within AWS Trusted Advisor?::It could check for an you are hitting any AWS service limits and quotas

What is the purpose of AWS Access Analyzer?::It can be used to check the fine-grained permissions of your AWS Identity and Access Management

How does AWS License Manager help with reducing the risk of non-compliance?::It can help reduce the risk of non-compliance by enforcing license usage limits, blocking new launches and using other controls

What is the purpose of the Amazon Customer Compliance Center?::It provides you with resources to help you learn more about AWS compliance including answers to key compliance questions and an auditing security checklist

## Module 11 - Pricing and Support
What does the pricing model of "Pay as you go" mean with regards to AWS?::You only pay for resources you actually consume, without any upfront costs or long-term commitments

What is the pricing concept of "Save when you commit" with regards to AWS?::It means that by committing to a certain level of usage for a period of time which is usually 1 or 3 years, you receive a pretty significant discount

What is the pricing concept of "Pay less by using more" with regards to AWS?::It means that due to the economy of scale, there can be volume based discounts where as the usage of the service increases, you begin to benefit from the economy of scale

For services like EC2, Lambda, and ECS, what metric are you charged based on with regards to AWS pricing?::You are charged based on the processing power and time used

For services like S3, EBS, what metric are you charged based on with regards to AWS pricing?::You are charged based on the amount of data stored and for how long

For Amazon S3 storage, what are the 6 cost components when storing and managing customer data?::1) Storage pricing, 2) request and data retrieval pricing, 3) data transfer and transfer acceleration pricing, 4) data management and analytics pricing, 5) replication pricing, and 6) price to process your data with Amazon S3 object lambda

When would you not incur a charge for data transfer with regards to AWS pricing?::You do not incur a charge for data transfer between AWS services within the same region

When would you incur a charge for data transfer with regards to AWS pricing?::You would incur a charge in some exception case for transfers within the same region as well as for outbound data transfer 

What does the AWS Billing Dashboard show you?::It shows you an overview of the forecasted spend for the month, and a breakdown of your most-consumed services by cost

What does the AWS Budgets service allow you to do?::You can create custom budgets to track your cost and usage. Additionally budgets can be generated for specific services, cost categories, or even by custom tags added for services you designate

What does the AWS Cost Explorer allow you to do?::It allows you to review costs and usage over time and allows you to break down cost by service, linked account , and also tags

What does the AWS Pricing Calculator provide?::It provides a web-based planning tool that you can use to create estimates for pricing for specific configurations such as instance types, storage options, and data transfer volumes

What are the 4 levels of support plans available in AWS?::1) Basic, 2) Develop, 3) Business, and 4) Enterprise

What is the response time for when a system is impaired between the 4 support levels?::1) 12 hours, 2) 4 hours, 3) 4 hours, 4) 4 hours

What is the response time for when a system is down between the 4 support levels?::1) 12 hours, 2) 1 hour, 3) 30 minutes, 4) 15 minutes

What is AWS re:Post?::It is a community driven, question-answer platform where uses can seek help, share knowledge, and find solutions related to AWS services

What is the purpose of AWS Trust and Safety Center?::It provides information on how to report activity or content on AWS that you suspect is abusive

What is the purpose of AWS Solution Architects?::They provide architectural guidance, best practice recommendations and help in designing scalable and secure applications

What is the purpose of AWS Professional Services?::It is a consulting service that offers deeper, project-based support. 

What are 3 examples of help that AWS Professional Services provides?::They provide help with 1) complex migrations, 2) security audits and 3) performance tuning

Which support levels would include direct technical assistance for guidance?::Developer support level and above

What is the AWS Marketplace?::It is a curated digital catalog that one can use to find, test, buy, deploy and manage third-party software running in your AWS architecture

What are the 3 categories of solutions and services provided on the AWS Marketplace?::1) Software as a service, 2) Machine Learning (ML) and AI, and 3) Data and Analytics

What does APN stand for with regard to AWS communities?::It stands for AWS Partner Network

What is APN with regards to AWS communities?::It is a global community that uses AWS technologies, programs, expertise and tools to build solutions and services for customers

What are the 3 benefits of being a AWS Partner?::There are 1) Funding Benefits, 2) AWS Partner events like webinars, virtual workshops, and in-person learning opportunities, and 3) AWS Partner Training and Certifications

What is the concept of right-sizing when it comes to cost optimization?::It is the idea where you are analyzing and adjusting your resources to match the needs of your workload

How can auto scaling help with cost optimization?::It can help by automatically removing any excess resource capacity when demand drops so that you avoid overspending

How can read replicas on RDS help with cost optimization?::Read replicas help with cost optimization by providing the ability to scale horizontally which spreads out the read capacity instead of requiring the vertical scaling up of the primary instance

How can caching on RDS help with cost optimization?::Caching helps with cost optimization by reducing the load on the primary instance

How can a customer save on cost for text files that are rarely accessed?::It is possible to configure a Lambda function to automatically compress the content and even add a lifecycle policy to delete old versions of objects

How can VPC endpoints be used for cost optimization?::It can be used as a way to privately connect your AWS VPC to supported AWS services without using the public internet which would cut down on data transfer costs

## Module 12 - Migrating to the AWS Cloud