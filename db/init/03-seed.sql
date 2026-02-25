-- 03-seed.sql
USE `photosi`;

-- USERS
INSERT INTO `users` (`id`, `email`, `password_hash`, `phone_number`, `first_name`, `last_name`, `created_at`)
VALUES
  (1, 'admin@demo.it', 'YWRtaW4=', '+391111111111', 'Admin', 'Demo', CURRENT_TIMESTAMP(6)),
  (2, 'user@demo.it',  'dGVzdA==',  '+392222222222', 'User',  'Demo', CURRENT_TIMESTAMP(6));

-- ADDRESSES
INSERT INTO `addresses` (`id`, `user_id`, `country_code`, `city`, `postal_code`, `street`, `created_at`)
VALUES
  (1, 1, 'IT', 'Roma',   '00100', 'Via Roma 1', CURRENT_TIMESTAMP(6)),
  (2, 2, 'IT', 'Milano', '20100', 'Via Milano 2', CURRENT_TIMESTAMP(6));

-- CATEGORIES
INSERT INTO `categories` (`id`, `name`)
VALUES
  (1, 'Photo'),
  (2, 'Frames'),
  (3, 'Gifts');

-- PRODUCTS
INSERT INTO `products` (`id`, `category_id`, `sku`, `name`, `description`, `price`)
VALUES
  (1, 1, 'PH-001', 'Photo Print 10x15', 'Standard 10x15 print', 0.50),
  (2, 1, 'PH-002', 'Photo Print 20x30', 'Standard 20x30 print', 1.50),

  (3, 2, 'FR-001', 'Classic Frame', 'Classic black frame', 9.99),
  (4, 2, 'FR-002', 'Wood Frame',    'Natural wood frame', 14.99),

  (5, 3, 'GF-001', 'Photo Mug',      'Mug with photo print', 7.99),
  (6, 3, 'GF-002', 'Photo T-Shirt',  'T-Shirt with photo',   12.99);