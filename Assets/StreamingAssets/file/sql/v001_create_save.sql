CREATE TABLE save_metadata (
    singleton_id INTEGER PRIMARY KEY CHECK (singleton_id = 1),
    schema_version INTEGER NOT NULL,
    base_content_version INTEGER NOT NULL,
    created_utc_seconds INTEGER NOT NULL,
    updated_utc_seconds INTEGER NOT NULL,
    logical_utc_seconds INTEGER NOT NULL
);

CREATE TABLE owned_equipment (
    equipment_id TEXT NOT NULL PRIMARY KEY,
    acquired_utc_seconds INTEGER NOT NULL
);

CREATE TABLE equipped_items (
    slot_type TEXT PRIMARY KEY CHECK (slot_type IN ('weapon', 'shield', 'accessory')),
    equipment_id TEXT NOT NULL UNIQUE,
    FOREIGN KEY (equipment_id) REFERENCES owned_equipment(equipment_id)
);

CREATE TABLE active_rumble (
    singleton_id INTEGER PRIMARY KEY CHECK (singleton_id = 1),
    started_utc_seconds INTEGER NOT NULL,
    completes_utc_seconds INTEGER NOT NULL,
    weapon_id TEXT NOT NULL,
    shield_id TEXT NOT NULL,
    accessory_id TEXT NOT NULL,
    location_id TEXT NOT NULL,
    photo_variant_id TEXT NOT NULL,
    reward_equipment_id TEXT NULL
);

CREATE TABLE album_photos (
    photo_id INTEGER PRIMARY KEY AUTOINCREMENT,
    acquired_utc_seconds INTEGER NOT NULL,
    expires_utc_seconds INTEGER NOT NULL,
    weapon_id TEXT NOT NULL,
    shield_id TEXT NOT NULL,
    accessory_id TEXT NOT NULL,
    location_id TEXT NOT NULL,
    photo_variant_id TEXT NOT NULL
);

CREATE TABLE discovered_locations (
    location_id TEXT NOT NULL PRIMARY KEY,
    first_discovered_utc_seconds INTEGER NOT NULL
);
