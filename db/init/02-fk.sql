-- 02-fk.sql
USE `photosi`;

-- products -> categories (Non si può cancellare una Category se c'è almeno un Product che ne ha il riferimento)
ALTER TABLE `products`
ADD CONSTRAINT `fk_products_categories_category_id`
FOREIGN KEY (`category_id`) REFERENCES `categories` (`id`)
ON DELETE RESTRICT ON UPDATE CASCADE;

-- orders -> users (Non si può cancellare uno User se c'è almeno un Order che ne ha il riferimento)
ALTER TABLE `orders`
ADD CONSTRAINT `fk_orders_users_user_id`
FOREIGN KEY (`user_id`) REFERENCES `users` (`id`)
ON DELETE RESTRICT ON UPDATE CASCADE;

-- order_items -> products (Non si può cancellare un Product se c'è almeno un OrderItem che ne ha il riferimento)
ALTER TABLE `order_items`
ADD CONSTRAINT `fk_order_items_products_product_id`
FOREIGN KEY (`product_id`) REFERENCES `products` (`id`)
ON DELETE RESTRICT ON UPDATE CASCADE;

-- Evita duplicati (Non è possibile inserire lo stesso prodotto 2 volte nello stesso ordine)
ALTER TABLE `order_items`
ADD UNIQUE KEY `ux_order_items_order_id_product_id` (`order_id`, `product_id`);