# Event Management System

An ASP.NET Core event management API that allows users to create, view, update, and delete event categories, events, and ratings. This system implements role-based authorization using JWT to secure endpoints, ensuring only authenticated users with appropriate roles can perform specific actions. Users can register as event organizers, manage their events, and leave ratings, while admins have control over all resources.

Features:

    Event Categories: Create and manage different events of categories.
    Events: Organize events within categories, with options for detailed descriptions, schedules, and pricing.
    Ratings: Add and manage event ratings.
    Secure Endpoints: JWT-based authentication with user roles and permissions to restrict actions by role.
    Scalability: Built to handle future extensions with a modular, RESTful API design.

This project follows REST principles and can be extended for various event management functionalities.
