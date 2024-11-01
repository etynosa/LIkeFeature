---

# Documentation for the “Like” Button Feature

## Overview

The "Like" button feature allows users to express their appreciation for articles by clicking a button, which increments a like count associated with the article. This feature is designed for ease of integration into any article, providing both visibility of the total likes and an interactive way for users to engage with content.

### Use Cases
1. **Display Total Likes**: When a user visits an article, the Like button is rendered, showing the total number of likes the article has received.
2. **Increment Likes**: When a user clicks the Like button, the like count increments by one.

## System Architecture

### Client-Side
- A frontend component (e.g., a React button) that displays the current like count and handles user interactions.
- The component should make API calls to retrieve the like count and to submit new likes.

### Server-Side
- An API built with .NET 6 that handles requests for retrieving like counts and adding likes.
- Uses a SQLite database for persistence of like data.
- Implements caching to optimize read performance and reduce database load.

### Database Structure
- **Table**: `ArticleLikes`
  - **Columns**:
    - `Id` (Primary Key, INT)
    - `ArticleId` (VARCHAR)
    - `UserId` (VARCHAR)
    - `CreatedAt` (DATETIME)

###  Live Url
- https://likefeature.onrender.com

###  Key API Endpoints
- `GET /api/articlelike/{articleId}/count`: Retrieves the total like count for a specified article.
- `POST /api/articlelike/{articleId}/like`: Increments the like count for a specified article.
- `GET /api/articlelike/{articleId}/hasLiked`: Checks if the user has already liked the article.

This solution for the “Like” button feature is not just functional; it’s built with a comprehensive set of architectural principles that prioritize scalability, performance, security, and resilience. Here’s a detailed breakdown of the key aspects that make this approach stand out:

### Key Aspects of the Solution

#### Scalability Features

1. **Redis Caching with TTL**: 
   - Using Redis for caching like counts significantly reduces database load. The 5-minute TTL ensures that the cache is refreshed regularly while balancing performance and data freshness.

2. **Response Caching**: 
   - Caching GET request responses for 60 seconds minimizes unnecessary calls to the database and improves response times for users viewing articles.

3. **Distributed Locking**: 
   - Implementing distributed locking prevents race conditions when multiple users attempt to like an article simultaneously. This ensures the integrity of the like count.

4. **Database Indexing**: 
   - Indexes on `ArticleId` and `UserId` optimize query performance, allowing for faster retrieval and updates, especially as the dataset grows.

5. **Optimized Async Queries**: 
   - Using asynchronous operations (`async/await`) throughout the application ensures that the server can handle multiple requests concurrently without being blocked, enhancing responsiveness.

#### Anti-Abuse Measures

1. **Rate Limiting**: 
   - Enforcing a limit of 10 likes per hour per user deters spam and malicious activity, promoting fair usage.

2. **User Fingerprinting**: 
   - Combining IP and User-Agent hashes creates a unique identifier for users, which aids in identifying and blocking potential abuse while preserving user privacy.

3. **Duplicate Like Prevention**: 
   - Implementing database constraints ensures that duplicate likes from the same user on the same article are not recorded, maintaining the accuracy of the like count.

4. **Distributed Locking for Requests**: 
   - This mechanism ensures that even under heavy load, rapid-fire requests do not result in conflicting updates to the like count.

#### High Availability Design

1. **Circuit Breaker Pattern**: 
   - Although not shown in detail, implementing this pattern helps to prevent cascading failures in the event of a service outage, allowing for graceful degradation.

2. **Graceful Error Handling**: 
   - Comprehensive error handling and logging ensure that failures are managed smoothly without crashing the application, providing a better user experience.

3. **Cache-Aside Pattern**: 
   - This approach enables efficient read operations by checking the cache before querying the database, reducing latency and load.

4. **Write-Through Caching**: 
   - Updates to the like count go through the cache first, ensuring that reads are always up-to-date without direct reliance on the database for every like.

#### Performance Considerations

1. **Asynchronous Operations**: 
   - Throughout the application, async operations minimize the chance of bottlenecks, particularly under high concurrency scenarios.

2. **Minimal Database Round Trips**: 
   - The use of caching strategies reduces the number of trips to the database, which is crucial for performance in high-load environments.

3. **Efficient Caching Strategy**: 
   - A well-planned caching strategy (using both Redis and response caching) maximizes throughput and minimizes latency.

4. **Optimized Database Schema**: 
   - Designing a schema that supports fast reads and writes aligns with the goal of maintaining performance as usage scales.

---
## Bonus Challenge Solution

### Handling Millions of Concurrent Users

#### Read Scaling (Getting Like Count)

1. **Redis Cache for Reads**: 
   - The primary cache layer handles most read requests efficiently, ensuring that the system remains responsive under load.

2. **Response Caching**: 
   - This strategy reduces redundant load on the cache by ensuring that frequently requested data is quickly served.

3. **Redis Read Replicas**: 
   - For further scaling, read replicas can be added to distribute read requests, improving response times and reliability.

4. **CDN Integration**: 
   - A Content Delivery Network (CDN) can offload static content, further optimizing performance and reducing the burden on the backend.

#### Write Scaling (Adding Likes)

1. **Redis for Write Throughput**: 
   - The high-performance nature of Redis allows it to handle a substantial volume of writes efficiently, accommodating spikes in usage.

2. **Distributed Locking for Writes**: 
   - This ensures that concurrent writes do not lead to inconsistent states in the database.

3. **Database Sharding**: 
   - Sharding the database by `ArticleId` can distribute writes across multiple nodes, further enhancing write performance.

4. **Event Sourcing**: 
   - Implementing event sourcing could enable the system to manage like events in a way that enhances scalability and enables rich event tracking.

---

### Additional Improvements

1. **Event Sourcing for Like Events**: 
   - This would allow for a complete history of likes, enabling features like undoing a like or analyzing user engagement patterns.

2. **Background Jobs for Reconciliation**: 
   - Periodic background jobs can reconcile the state of likes with the database, ensuring accuracy without impacting performance.

3. **WebSocket Support for Real-Time Updates**: 
   - Real-time updates via WebSockets would allow users to see live like counts without refreshing the page.

4. **Geographic Distribution of Redis Instances**: 
   - Distributing Redis instances globally can enhance performance for users by reducing latency in data access.

---

### Monitoring and Observability

1. **Metrics Tracking**: 
   - Tracking cache hit/miss ratios helps in fine-tuning the caching strategy, ensuring optimal performance.

2. **Rate Limit Monitoring**: 
   - Keeping track of rate limit rejections allows for adjustments in thresholds or rules to optimize user experience.

3. **Performance Monitoring**: 
   - Monitoring Redis and database performance metrics provides insights for proactive scaling and issue resolution.

4. **Anomaly Alerts**: 
   - Setting up alerts for unusual patterns can help detect and mitigate potential abuse or system failures early.

---

### Security Enhancements

1. **Authentication Middleware**: 
   - Securing endpoints with authentication ensures that only authorized users can perform certain actions, like liking articles.

2. **Request Validation**: 
   - Validating incoming requests prevents malicious data from being processed by the server.

3. **CORS Policies**: 
   - Implementing proper Cross-Origin Resource Sharing (CORS) policies protects against unauthorized access from other origins.

4. **Rate Limiting by IP Range**: 
   - Further enhancing security by preventing excessive requests from a single IP range.

---

### Further Scale Considerations

1. **Database Partitioning Strategy**: 
   - A solid partitioning strategy ensures that data is distributed effectively across database servers, improving performance.

2. **Redis Cluster Configuration**: 
   - Configuring a Redis cluster allows for better performance and scalability, as data is distributed across multiple Redis instances.

3. **CDN Integration**: 
   - As mentioned, CDNs can significantly reduce load on the backend, particularly for static resources.

4. **Load Balancer Setup**: 
   - Using a load balancer ensures even distribution of traffic across multiple API instances, improving reliability and performance.

---





## Repository
- The complete implementation can be found in the following GitHub repository: https://github.com/etynosa/LIkeFeature

