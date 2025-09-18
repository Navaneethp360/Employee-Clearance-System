# Phone Directory System

A fast, web-based internal **Phone Directory** for employees, designed for rapid search and display of contact information. The system integrates **Active Directory (AD)** with caching for performance, and also supports **custom contacts** for internal extensions or non-AD employees.

---

## Features

- **Lightning-Fast Search & Filtering**
  - Instantly search employees by name, department, or company, even with tens of thousands of records.
  - Results are ranked for relevance: exact matches appear first, followed by partial matches.
  - Dynamic search-as-you-type updates **without server round-trips**, using client-side cached data.

- **Company Tabs**
  - Employees are grouped by company using tabbed navigation.
  - Quickly switch between companies without reloading the page.

- **Active Directory Integration**
  - Pulls employee details directly from AD.
  - Cached as JSON to reduce repeated AD queries and speed up searches.
  - Handles company, department, and role hierarchies.

- **Custom Contacts**
  - Store additional contacts not present in AD.
  - Managed via SQL tables.
  - Fully integrated with AD results in search output.

- **Web Service (ASMX)**
  - Exposes directory data via an **ASMX service**.
  - Initially fetches AD and custom contact data.
  - Returns combined JSON results to the client for caching.
  - Subsequent searches and filtering are performed **entirely on the client side**, avoiding repeated server requests and ensuring near-instant results.

- **UI & User Experience**
  - Responsive design with clean GridView/list rendering.
  - Search-as-you-type functionality powered by client-side cache.
  - Dynamic tabs for company selection.
  - Supports both **Light Mode** and **Dark Mode** switching based on user preference.
  - Optimized rendering to maintain performance even with large datasets.

- **Caching**
  - **Client-Side Caching:** Once data is loaded from the ASMX service, all subsequent searches and filters are executed in the browser using the cached JSON.
  - Reduces server load and improves responsiveness.
  - Cache is refreshed periodically when the page reloads or when the administrator updates AD/custom contacts.

---

## Technology Stack

- **Frontend:** ASP.NET Web Forms, HTML, CSS, JavaScript
- **Backend:** C#, .NET Framework, ASMX Web Service
- **Database:** SQL Server (for custom contacts and caching metadata)
- **Authentication:** Active Directory (AD)

---

## Architecture Overview

1. **Data Source**
   - AD data fetched periodically and stored as JSON.
   - Custom contacts stored in SQL tables.

2. **Web Service (ASMX)**
   - Receives search/filter requests from the frontend.
   - Queries cached AD JSON and custom contact tables.
   - Returns combined JSON results to the client.

3. **Client Rendering & Caching**
   - JSON data returned from the ASMX service is **cached on the client**.
   - JavaScript performs all searches, filters, and tab switching using this cache.
   - No additional server calls required for each search, making the system **lightning fast**.
   - Company tabs, search ranking, and dynamic filtering all operate on the cached data.
   - Cache refreshes only when data is updated or the page is reloaded.

---
