
## Google Calendar API .NET Core Application Documentation

### API Endpoints

#### 1. Create Event
- **Endpoint:** `POST /api/events`
- **Description:** Allows users to create a new event in their Google Calendar.
- **Input:**
    - JSON object containing event details including `summary`, `description`, `start`, and `end` fields.
    - Date and time format for `start` and `end`: "yyyy-MM-ddTHH:mm:ss" (e.g., "2023-12-31T09:00:00").
- **Output:**
    - Details of the created event along with HTTP status code 201 (Created).
- **Possible Errors:**
    - 400 Bad Request: Invalid input or missing required fields.
    - 401 Unauthorized: Authentication issues with Google Calendar API.
    - 403 Forbidden: Creating events in the past or invalid access.
    - 5xx Server Errors: Unexpected server errors.

#### 2. View Events
- **Endpoint:** `GET /api/events`
- **Description:** Retrieves a list of events from the user's Google Calendar.
- **Input:**
    - Optional query parameters:
        - `startDate` (start date filter)
        - `endDate` (end date filter)
        - `searchQuery` (search for specific events)
- **Output:**
    - List of events matching the filter criteria.
- **Possible Errors:**
    - 401 Unauthorized: Authentication issues with Google Calendar API.
    - 403 Forbidden: Access issues or invalid query parameters.
    - 5xx Server Errors: Unexpected server errors.

#### 3. Delete Event
- **Endpoint:** `DELETE /api/events/{eventId}`
- **Description:** Removes an event from the user’s Google Calendar.
- **Input:**
    - `eventId` as a URL parameter.
- **Output:**
    - HTTP status code 204 (No Content) upon successful deletion.
- **Possible Errors:**
    - 400 Bad Request: Invalid or missing `eventId`.
    - 401 Unauthorized: Authentication issues with Google Calendar API.
    - 404 Not Found: Event not found or couldn't be deleted.
    - 5xx Server Errors: Unexpected server errors.

### Setup and Running the Application

1. **Clone Repository:**
   - Clone the repository to your local environment.

2. **Google API Credentials:**
   - Go to the [Google Developer Console](https://console.developers.google.com/) and create a project.
   - Enable the Google Calendar API for the project.
   - Create OAuth 2.0 credentials (client ID and client secret).
   - Configure redirect URIs (e.g., `http://localhost:PORT/callback`) for authorization.
   - Download the credentials as a JSON file (`credentials.json`).

3. **Application Setup:**
   - Set up your environment variables or appsettings to store necessary configurations like client ID, client secret, redirect URIs, and other sensitive data.

4. **Run the Application:**
   - Run the .NET Core application using your preferred IDE or command line.
   - Test the endpoints using API testing tools like Postman or Swagger UI.

5. **Authorization:**
   - Use the application to authorize Google Calendar access.
   - Ensure that the authorized user has permission to create, view, and delete events in their Google Calendar.

### Important Notes
- Ensure proper error handling and logging for API requests and responses.
- Protect sensitive information such as API keys and secrets.
- Handle authentication and authorization securely, following Google's API best practices.

Adjust the instructions based on the specifics of your project, environment, and any additional functionalities or security measures required.
