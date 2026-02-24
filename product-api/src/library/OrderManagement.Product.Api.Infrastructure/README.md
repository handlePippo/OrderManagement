** FK products.category_id -> categories.id **

* Serve a impedire la creazione\aggiornamento di un prodotto con una categoria inesistente.

ALTER TABLE products
ADD CONSTRAINT fk_products_category_id
FOREIGN KEY (category_id)
REFERENCES categories(id)
ON DELETE RESTRICT
ON UPDATE RESTRICT;