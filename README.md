# to-do-task-management-api

to-do-task-management-api
A backend API designed for managing To-Do tasks.
It provides endpoints for creating, updating, deleting, and retrieving tasks, and is intended to be consumed by a frontend UI (such as the Vue.js To-Do application).

How to Run the API
1. Open the Solution

Open the project solution in Visual Studio.

2. Configure CORS

Open the appsettings.json file.

Update the AllowedCorsOrigins key to include the URL of your local UI application.

Example:

"AllowedCorsOrigins": [
  "http://localhost:5173"
]

3. Run the Application

Start the API from Visual Studio (using IIS Express or Kestrel).

The Swagger UI will automatically open in your browser.

You can use Swagger to:

Test API endpoints

View request/response formats

Verify successful communication with your frontend


Assumptions

Tasks belong to a single user (no multi-tenant support).

Basic CRUD operations are enough; no advanced features like categories or due dates.

No authentication/authorization is required.

Using an in-memory or simple local database (SQL Lite / SQL Server).

API and UI run on different ports; therefore CORS is required.


Future Enhancements (API)
Feature				Description
User authentication		JWT tokens, refresh tokens
Multi-user support		Assign tasks per user
Task categories / labels	Group tasks meaningfully
Due dates & reminders		Schedule reminders
Pagination & filtering APIs	/tasks?status=completed&search=xyz
Bulk operations			Bulk delete, bulk update
Audit logging			Track task edit history
Background jobs			Email reminders, cleanup tasks
GraphQL endpoint		Optional richer querying
