# Museum API Full-Stack with React & .NET 7

###### ASP.NET Core 7.0.4
###### Node.js 18.15.0, React with TypeScript, Functional Components and Vite
###### Backend deployed on Microsoft Azure, Frontend deployed on Netlify


## 1

Create a REST API with CRUD functionalities that manages a Museum.

You will need to:
- Add at least `3` entities in total.
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
- Deploy your project to a virtual machine. Your application should be usable by anyone with a link to it. You must use a remote server that you can SSH into. You are not allowed to use local VMs and you are not allowed to use your own home server.
- Add at least `3` validation rules. They can be spread across multiple entities.

## 4
- Add a frontend to your REST API. It should be a Single Page Application (SPA).
- Add Swagger to your REST API. It should be publicly accessible.
- Implement at least one CRUD and one filter / statististical report on the frontend.
- Users should be able to navigate the interface without manually editing the URL. The interface should also be intuitive and self-explanatory: try adding a UI Components Library like Material UI, Bulma, or even something like Bootstrap, Tailwind etc.
- Deploy the frontend either on https://www.netlify.com/.


