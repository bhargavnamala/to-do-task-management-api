# to-do-task-management-api

🚀 How to Run the API
1. Open the Solution

Open the project solution in Visual Studio.

2. Configure CORS

Open the appsettings.json file and update the AllowedCorsOrigins key to include the URL of your UI application.

Example:

"AllowedCorsOrigins": [
  "http://localhost:5173"
]

3. Run the Application

Start the API using IIS Express or Kestrel from Visual Studio.

Once the application starts:

Swagger UI will open automatically.

You can use Swagger to:

Test API endpoints

View request/response formats

Verify communication between the UI and API

📌 Assumptions

Tasks belong to a single user (no multi-tenant support).

Basic CRUD operations are sufficient (no categories, labels, or due dates).

No authentication or authorization is required.

Uses an in-memory or simple local database (SQLite / SQL Server).

API and UI run on different ports; CORS is required.

🔮 Future Enhancements (API)
Feature	Description
User authentication	JWT tokens, refresh tokens
Multi-user support	Assign tasks per user
Task categories / labels	Organize tasks more meaningfully
Due dates & reminders	Schedule notifications
Pagination & filtering	/tasks?status=completed&search=xyz
Bulk operations	Bulk update/delete
Audit logging	Track task edit history
Background jobs	Email reminders, cleanup tasks
GraphQL endpoint	Optional richer querying
