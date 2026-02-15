# Intern/Junior Developer Evaluation Project {#internjunior-developer-evaluation-project .unnumbered}

#  {#section .unnumbered}

**Project Specification Document**

**Project Title:** MiniRent -- Apartment & Property Rental Management
System **Company:** DAFTech Social Solution **Project Objective:** This
project serves as an entrance evaluation for interns and junior
developers applying to join DAFTech Social Solution. It assesses
candidates\' skills in building a full-stack web application, including
understanding of clean architecture, separation of concerns, dependency
injection, API design, frontend development, database management, and
problem-solving. The project must be implemented within a strict 1-week
timeframe to simulate real-world deadlines. Candidates will be evaluated
based on code quality, completeness, functionality, and adherence to
best practices.

**Target Audience:** Small real estate offices or independent agents
managing apartment rentals (e.g., 50--200 units). The system focuses on
property listing, rental tracking, and inquiry management.

**Estimated Implementation Time:** 7 days (full-time effort, assuming
6--8 hours/day). Candidates should prioritize MVP features and
demonstrate progressive development.

# Submission Guidelines: {#submission-guidelines .unnumbered}

-   Submit a GitHub repository (public or shared access) with clear
    README.md including setup instructions, database schema, and demo
    steps.

-   Use branches for features (e.g., feature/auth,
    feature/property-management).

# Technology Stack

-   **Backend:** .NET Core 8 (ASP.NET Core Web API)

-   **Frontend:** Angular 18 (with TypeScript)

-   **Database:** MS SQL Server (use LocalDB for development; provide
    migration scripts for production).

-   **Authentication:** JWT (JSON Web Tokens) for secure API access.

-   **ORM:** Entity Framework Core for database interactions.

# Additional Tools/Libraries:

-   Backend: AutoMapper (for DTO mapping), Serilog (basic logging).

-   Frontend: Angular Material or Bootstrap for UI (keep it simple).

-   Testing: Optional unit tests with xUnit (bonus).

```{=html}
<!-- -->
```
-   **Deployment:** Local setup required; optional Docker compose for
    bonus points.

# Functional Requirements

The system must support core operations for managing apartment rentals.
Requirements are categorized by module, with detailed functionalities
listed. All APIs must be RESTful, and frontend must consume them
securely.

# Authentication & User Management

-   **Register New User:** Allow admins to create agent accounts
    (username, password, full name, role). Students/interns should
    implement password hashing (e.g., BCrypt).

-   **Login:** Authenticate users and return JWT token with role claims.
    Token expiration: 1 hour.

-   **Logout:** Client-side token removal (no backend endpoint needed).

-   **Get Current User Profile:** Retrieve logged-in user\'s details
    (username, role).

-   **Role-Based Access:** Admins can manage everything; Agents can view
    and add rentals/inquiries but not delete properties.

# Property Management

-   **List All Properties:** Display paginated list (10 items/page) with
    filters (status, bedrooms, price range, address search). Sort by
    rent ascending/descending.

-   **View Property Details:** Show full details including address, area
    (sqm), bedrooms, floor, monthly rent, status, and rental history
    (last 3 rentals).

-   **Add New Property:** Create with validation (e.g., rent \> 0,
    bedrooms \>= 1).

-   **Edit Property:** Update details; audit who edited (bonus: add
    CreatedBy/UpdatedBy fields).

-   **Delete Property:** Soft delete (set IsDeleted flag); prevent
    deletion if currently rented.

-   **Change Property Status:** Toggle between Available, Rented,
    Reserved, Maintenance.

# Rental Management

-   **List Rentals:** Show active and historical rentals, filtered by
    property or date range.

-   **Create New Rental:** For a property, add tenant details (name,
    phone, start date, deposit, monthly rent). Auto-set property status
    to Rented.

-   **View Rental Details:** Display full rental info including end date
    (if set) and notes.

-   **End Rental:** Set end date, calculate total paid (bonus: simple
    formula based on months), and revert property to Available.

-   **Update Rental:** Edit tenant details or notes; cannot change start
    date after creation.

# Inquiry/Lead Management

-   **List Inquiries:** Paginated list with filters (status, date,
    property).

-   **Add New Inquiry:** From property detail or standalone; include
    name, phone, email, message. Auto-set status to New.

-   **View Inquiry Details:** Show full info and linked property.

-   **Update Inquiry Status:** Change to Contacted, Rejected, or
    Converted (to rental).

-   **Delete Inquiry:** Hard delete for old inquiries.

# Dashboard & Reporting

-   **Dashboard Overview:** Display key metrics:

    -   Total properties.

    -   Count by status (Available/Rented/etc.).

    -   New inquiries this month.

    -   Rental revenue summary (total monthly rent from active rentals).

-   **Search Functionality:** Global search across properties and
    inquiries (e.g., by address or tenant name).

# Non-Functional Requirements

-   **Security:**

    -   All APIs except login/register must require JWT authentication.

    -   Use \[Authorize\] attributes with roles.

    -   Validate inputs to prevent SQL injection/XSS (use built-in
        features).

    -   Store passwords hashed.

# Recommended Project Structure

To evaluate clean architecture and separation of concerns:

# Backend (.NET Core) {#backend-.net-core .unnumbered}

text

MiniRent.Backend/

├── **Controllers**/

│ ├── AuthController.cs

│ ├── PropertiesController.cs

│ ├── RentalsController.cs

│ ├── InquiriesController.cs

│ └── DashboardController.cs

├── **Services**/

│ ├── **Interfaces**/

│ │ ├── IAuthService.cs

│ │ ├── IPropertyService.cs

│ │ ├── IRentalService.cs

│ │ └── IInquiryService.cs

│ ├── AuthService.cs

│ ├── PropertyService.cs

│ ├── RentalService.cs

│ └── InquiryService.cs

├── **Models/ (Entities)**

│ ├── Property.cs

│ ├── RentalRecord.cs

│ ├── RentalInquiry.cs

│ └── User.cs

├── Dtos/

│ ├── PropertyDto.cs

│ ├── PropertyCreateDto.cs

│ └── \...

├── Data/

│ └── AppDbContext.cs

├── Program.cs

└── appsettings.json **Frontend (Angular)** text

MiniRent.Frontend/ src/app/

├── auth/ (login/register components)

├── properties/

│ ├── property-list/

│ ├── property-detail/

│ └── property-form/

├── rentals/

├── inquiries/

├── dashboard/

├── shared/

│ ├── services/ (api.service.ts, auth.service.ts)

│ ├── guards/ (auth.guard.ts, role.guard.ts)

│ └── components/ (navbar, error-dialog)

└── app-routing.module.ts

# 6. Evaluation Points & Criteria {#evaluation-points-criteria .unnumbered}

Candidates will be scored out of 100 points. Focus on demonstrating
fundamentals over perfection. Partial credit for incomplete but
well-structured work.

+----------------+------------------------+-----+---------------------+
| **Category**   | **Description**        | *   | **What We Look      |
|                |                        | *Po | For**               |
|                |                        | int |                     |
|                |                        | s** |                     |
+================+========================+=====+=====================+
| **             | All MVP                | 30  | Happy path flows    |
| Completeness** | functionalities        |     | (e.g.,              |
|                | implemented and        |     |                     |
|                | working end-to-end.    |     | add property → rent |
|                |                        |     | → end rental). No   |
|                |                        |     | major missing       |
|                |                        |     | features.           |
+----------------+------------------------+-----+---------------------+
| **Code Quality | Clean architecture     | 20  | No business logic   |
| & Structure**  | (controllers thin,     |     | in controllers.     |
|                | services handle logic, |     | Proper naming, no   |
|                | repos for data). Use   |     | code                |
|                | of interfaces, DI.     |     |                     |
|                |                        |     | duplication.        |
+----------------+------------------------+-----+---------------------+
| **Database &   | Correct schema,        | 15  | Use EF Core         |
| ORM**          |                        |     | fluently.           |
|                | relationships,         |     |                     |
|                | migrations. Efficient  |     | Indexes on search   |
|                | queries (no N+1        |     | fields (bonus).     |
|                | issues).               |     |                     |
+----------------+------------------------+-----+---------------------+
| **Frontend**   | Responsive UI, proper  | 15  | Components modular, |
|                | state management       |     | services for HTTP,  |
|                | (e.g., NgRx optional), |     | guards for auth.    |
|                | API integration with   |     |                     |
|                | error handling.        |     |                     |
+----------------+------------------------+-----+---------------------+
| **Security &   | JWT implemented        | 10  | No plain passwords, |
| Best           | correctly, input       |     | role checks, HTTPS  |
| Practices**    | validation, error      |     | setup               |
|                | logging.               |     |                     |
|                |                        |     | (local).            |
+----------------+------------------------+-----+---------------------+
| **Testing &**  | Unit tests, Swagger,   | 5   | At least basic      |
|                | README with            |     | tests; clear        |
| **D            | setup/demo.            |     | instructions.       |
| ocumentation** |                        |     |                     |
+----------------+------------------------+-----+---------------------+
| **Inno         | Nice-to-haves (e.g.,   | 5   | Extra features      |
| vation/Bonus** | search, photos),       |     | without breaking    |
|                | performance            |     | MVP.                |
|                |                        |     |                     |
|                | optimizations.         |     |                     |
+----------------+------------------------+-----+---------------------+

**Total: 100 Passing Threshold:** 70+ points. Top performers may receive
interview priority. Feedback will be provided on strengths/weaknesses.

This project aligns with DAFTech Social Solution\'s focus on practical,
scalable solutions. Good luck, and we look forward to reviewing your
submissions! If you have questions during implementation be free to
contact us
