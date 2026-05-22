CREATE KEYSPACE IF NOT EXISTS marketuees 
WITH replication = {'class': 'NetworkTopologyStrategy', 'DC1': 2};

USE marketuees;

CREATE TABLE IF NOT EXISTS product_views (
    product_id UUID,
    user_id UUID,
    view_date TIMESTAMP,
    PRIMARY KEY (product_id, view_date)
);

CREATE TABLE IF NOT EXISTS user_activity (
    user_id UUID,
    activity_date TIMESTAMP,
    activity_type TEXT,
    product_id UUID,
    PRIMARY KEY (user_id, activity_date)
);