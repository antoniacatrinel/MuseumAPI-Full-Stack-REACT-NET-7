# Museum API Full-Stack with React & .NET 7

###### ASP.NET Core 7.0.4
###### Node.js 18.15.0, React + Vite

## 1

Create a REST API with CRUD functionalities that manages Museums.

You will need to:
- Add at least `3` entities to your API.
- Implement CRUD for each of your entities.
- Implement filtering on a numeric field for at least one entity. The filtering should return all entities with the numeric field higher than a given value.
- A **1 to many** relation between two of your entities.
- Add persistent storage to a database. Rerunning the application should keep previously existing data.
- Have a REST API usable in Postman.

## 2
- Add a **many to many** relation between `2` entities. This should be added as a separate entity containing the two related entities or their IDs and at least 2 additional attributes. 
- Add at least `10` realistic-looking records for each of your models / tables.
- Use Trello and set up a Kanban board (with at least `Todo`, `Doing`, `Done` or equivalent lists).
- Create a UML diagram for your application and a database diagram for your database. 
- Add a statistical report involving two entities. Such reports **need** to show the computed field. You probably want to use a `DTO` class for this.

## 3
- Add unit tests for the two non-CRUD functionalities from the previous assignments using a "mocking library". 
- Deploy your project to a virtual machine on [Microsoft Azure](https://azure.microsoft.com/en-us) and [Google Cloud](https://cloud.google.com/free). Your application should be usable by anyone with a link to it. You must use a remote server that you can SSH into. You are not allowed to use local VMs and you are not allowed to use your own home server.
- Add at least `3` validation rules. They can be spread across multiple entities.

## 4
- Add a frontend to your REST API. It should be a Single Page Application (SPA).
- Add Swagger to your REST API. It should be publicly accessible.
- Implement at least one CRUD and one filter / statististical report on the frontend.
- Users should be able to navigate the interface without manually editing the URL. The interface should also be intuitive and self-explanatory: try adding a UI Components Library like Material UI, Bulma, or even something like Bootstrap, Tailwind etc.
- Deploy the frontend on [Netlify](https://www.netlify.com/).

## 5
- Use an actual database server like MS SQL.
- Populate your database with at least `1 000 000` (one million) records for each entity. Add `10 000 000` (ten million) records for the entity acting as an intermediary in the many to many relation. This should be done through SQL scripts (one or more files) that you execute outside of your application. The data should be realistic looking. The script should execute in a reasonable amount of time. Some ideas and suggestions below: 
  - Although the end result needs to be at least one SQL script, you can use an external program (such as a Python script) that generates the SQL file by writing data in it. It might be easier to have things like lists that store base values in Python / other languages compared to pure SQL. Consider using the `Faker` library for this: https://faker.readthedocs.io/en/master/. 
  - You might want to batch the inserts to make things faster: `INSERT INTO tbl_name (a, b, c) VALUES (1, 2, 3), (4, 5, 6), (7, 8, 9);`. Batches of size `1000` generally work pretty well.
  - For the many to many intermediate table, make sure that you don't repeat the same pair of related IDs (unless this makes sense in your particular case).
- Implement an API that bulk adds entities to a **1 to many** relationship.
- Start using migrations: rerunning the application or changing something in your data models should **not** drop and recreate the database.

## 6
- Implement all functionalities on the frontend. Implement pagination on the backend and navigation through it on the frontend. You are **not allowed** to **only** use built-in pagination classes and pagination libraries. You must implement the pagination class or functionality yourself either on the frontend or on the backend. Do it in a way that allows for changes. For example, you may use built-in classes on the backend and no built-in classes on the frontend. Your live coding task during the lab will involve the pagination aspect.
- Make sure that your functionalities have efficient implementations. This may require adding indexes to your database, implementing autocomplete and fixing things on the backend. There shouldn't be a noticeable slowdown when interacting with any page.
- Each show all page should display, for each row, an aggregated value on a related entity (for example, the number of students enrolled in the course for the show all courses page). This should still be efficient and not affect page load times too much.
- Duplicate your validation logic on the frontend and add `2` more validation rules, also duplicated on the backend and the frontend.
- Error messages should show up on the frontend near the corresponding textboxes or using Toasters.
- Add autocomplete on related fields.
- If you haven't already, add a CSS Components Library to your frontend: Material UI, Bulma, Bootstrap, Tailwind or something similar.
- Secure and improve your server by:
    - Installing `nginx` and a dedicated application server for your backend. The `nginx` reverse proxy server should communicate with your application server and the application server should not be directly exposed to the internet. The application server can be Gunicorn, Apache, Tomcat or something else. For .NET projects, you can just use IIS or the default deployment scheme.
    - Installing an SSL certificate using something like `certbot` or `acmesh` and `freedns`. Since your VM IP will change if you shut down your VM, I recommend starting and configuring it well in advance of your lab, to account for any possible delays with the DNS propagation. Everything should now use `https`. Make sure you update your frontend accordingly.
    - Making both `nginx` and your application server services that start when your VM starts and that you control with commands such as `sudo systemctl start nginx`, `sudo systemctl start your_app_server`.

## 7
- Implement register and login with username, password and JWT tokens. The username should be unique. The password should have some validation rules to ensure that the password is strong.  
    - Your `User` model must contain any fields needed for login (probably just username and password and whatever else your framework has built in). You should also have a `UserProfile` model with `5` fields with at least 3 validation rules. For example: bio, location, birthday, gender, marital status.  
    - All of your entities should be directly or indirectly associated with the user that created them. Add `10 000` (ten thousand) random users and randomly associate the existing entities with these users. For testing purposes, have the same password for each of these users. Each entity **must** have an associated user, but not all users must have associated entities. The data insertion part must be implemented in an SQL script. You might want to work on a backup database before running the script on the production database.
    - The `/api/register` endpoint should generate a confirmation code that is valid for `10` minutes. The user must request `/api/register/confirm/<confirmation code>` to activate their account.
- Everything must be validated everywhere possible, including IDs. Implement the `happy case - with data`, `happy case - without data` and `error case` scenarios for all endpoints.
- For all routes that show all entities, also show the username of the user that added the entity. Clicking on the username should take you to the user's profile page.
    - The profile page should contain the user profile info and statistics regarding how many of each entity the user has added.
- You must start using feature branches for all functionalities. Your feature branches should be named according to the feature that you're implementing. Use Pull Requests and merge your branches into a `development` branch when you are done with your work. You can delete the feature branches after that. Have netlify deploy from `development`.

## 8
You will need to implement the following user roles:
- Anonymous user (no login): can only **read** everything, cannot add, cannot edit;
- Logged in user, **regular** role: can add entities and can edit the entities they added;
- Logged in user, **moderator** role: can add entities and can edit all entities;
- Logged in user, **admin** role: 
    - can add entities and can edit all entities, has access to a page where they can edit user roles for everyone including other admins;
    - has access to a page from where they can **bulk delete** data and from where they can **run the data generation scripts** you wrote for the previous assignments. If you find it easier, you can refactor them such that the data is generated directly from your code and not from the SQL scripts, but this is not mandatory.

Permissions need to be checked on both backend and frontend. 

If you haven't already, make sure that you do not store user passwords in plain text. They should at least by hashed with SHA-256.

- Add an editable field in the user profile page called **page preference** for each user. When the admin changes the page preference, all users' page preferences should change. Any other users can only modify his/hers page preference.

## 9
- Dockerize your application. You should have setups both for local development and production deployment. Make a new VM, install docker and docker-compose on it and deploy the project like that.
- Write two E2E tests for your application.
- Setup a Kubernetes Cluster on your Cloud Provider (for example: https://cloud.google.com/kubernetes-engine) and use your app with it. Use JMeter with the Ultimate Thread Group plugin and show how Kubernetes scales resources to accomodate requests from JMeter. Correlate the JMeter graphs with the Kubernetes graphs.
- Make your frontend responsive. It should seamlessly adapt to any screen size, with no scrollbars and with proper component sizes and layout.

## 10
- Make one or more graphs containing stress test (spike test) results: it should be clear from your graph(s) how the number of users affects the CPU usage and the request response times. Use JMeter. Start with few users: `~2` constant ones and `~10` for the spikes and gradually move up until you hit `100%` CPU usage. You should make requests to all the (public) endpoints of your API. You only need to do `GET` requests.
- Implement a chat page on your web app using websockets. Every visitor should be able to see messages from all other visitors. Let your visitors choose a nickname. You need to implement persistence for the messages.

## 11
- Integrate a machine learning model into your application in a reasonable way to make predictions. It can be trained on any data (your application's data or an external data set) and there should be a functionality in your application that makes use of your model's predictions. You can use any model you want.
