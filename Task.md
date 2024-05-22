### Objective

Your assignment is to implement a REST API for a restaurant.

### Brief

Frogo Baggins, a hobbit from the Shire, has a great idea. He wants to build a restaurant that serves traditional dishes from the world of Middle Earth. The restaurant will be called "**The Orc Shack**" and will have a cozy atmosphere.

Frogo has hired you to build the website for his restaurant. As payment, he has offered you either a chest of gold or a ring. Choose wisely.

### Tasks (Specifications)

This assignment has 4 tasks, which you can attempt based on your level of experience. We expect candidates applying for a Junior engineer positions to complete at least the first task, Intermediate engineers must also complete the second task, and finally, seniors must also complete the 3rd task. Lastly there are some ideas in the 4th task for engineers who want to go above and beyond.


#### Task 1 (All Candidates):

Deliver a REST API that meets the following requirements:
- An API user must be able to:
    - Create, View, List, Update, and Delete dishes.
    - Dishes must have a name, description, price, and image.

- Customers must be able to take the following actions:
    - Search, View, and Rate dishes

*Junior engineers do not _need_ to worry about users or authentication.*


#### Task 2 (Intermediate & Senior)
- Add user, permission, and authentication support.
- Users must be able to register and login.
- All functionality of the API must require a logged in user (except Registration)
- At a minimum, the system should support password based authentication.
- Users must have a name and email address and password.
- Add validation to the data entities in the API.
- An Evil Orc is attempting to brute force passwords for known email addresses. Add functionality to defend against this. (You can use any methodology that you deem suitable)


#### Task 3 (Senior)

- The API is running on an old Shire Server that is starting to struggle with the load of the now popular website. Implement a solution to improve the performance of the API on the same hardware. 
- Add support for multiple different restaurants to use the product (multi-tenant SaaS)


#### Task 4 (Above and Beyond)

- The evil Orc has created many sockpuppet accounts and has left many bad reviews. Use an AI/ML solution to provide a sentiment score for each review.
- To prevent abuse, add rate-limiting per logged in customer.
- Allow users to login using OAuth2 based SSO (Google, etc)

### Constraints

- At Bash we make extensive use of Golang so first prize will always be to use Golang for your assignment.
- Alternative languages we will accept are Python (preferably fastapi), or JS/Typescript
- Implement a REST API utilizing JSON for request and response bodies where applicable.

### Tips, Advice, Guidance

- You are encouraged to make use of a web framework, SQL ORM, etc. This will help reduce the overhead of writing boilerplate code, and will let you focus on the core requirements.
- You are welcome to make use of AI to help write the code.
- This assessment is open ended, and candidates could spend weeks crafting the perfect API. We encourage you to timebox yourself, and limit the amount of time you spend. When we talk through the assessment, this can be provided as an input, and it is good to talk about the trade-offs made given the time constraint. We recommend about 6-8 hours of focused time.

### Evaluation Criteria

The test will be evaluated based on functional and non-functional requirements.

For functional requirements, your API needs to work, and meet the requirements as provided for your level.

For non-functional requirements, your API needs to be production-ready to a reasonable extent. We are looking for adherence to qualities such as testability, maintainability, observability, and security.

### Supporting Assets

You've been provided with a docker-compose file which will bring up a postgres database and prometheus. These are optional and provided to help get started.

#### Postgres

You can connect to the database via localhost:5432 using the username and password configured in the docker-compose.yml.

#### Prometheus

You can configure prometheus via the provided prometheus.yml file.

### CodeSubmit

Please organise, design, test, and document your code as if it were going into production - then push your changes to the Main branch. After you have pushed your code, you may submit the assignment on the assignment page.

Best of luck, and happy coding!

The Bash Team
