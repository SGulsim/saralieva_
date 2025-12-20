--liquibase formatted sql

--changeset podcast:001-create-categories-table
CREATE TABLE IF NOT EXISTS categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    icon VARCHAR(100),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

--changeset podcast:001-create-categories-index
CREATE UNIQUE INDEX IF NOT EXISTS idx_categories_name ON categories(name);

--changeset podcast:002-create-users-table
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    email VARCHAR(256) NOT NULL,
    password_hash VARCHAR(500) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100),
    default_playback_speed DOUBLE PRECISION NOT NULL DEFAULT 1.0
);

--changeset podcast:002-create-users-index
CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email ON users(email);

--changeset podcast:003-create-podcasts-table
CREATE TABLE IF NOT EXISTS podcasts (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description VARCHAR(2000),
    rss_feed_url VARCHAR(500) NOT NULL,
    cover_image_url VARCHAR(500),
    category_id INTEGER,
    language VARCHAR(10) NOT NULL DEFAULT 'ru',
    last_updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_favorite BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_podcasts_category FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE SET NULL
);

--changeset podcast:003-create-podcasts-indexes
CREATE INDEX IF NOT EXISTS idx_podcasts_name ON podcasts(name);

--changeset podcast:004-create-episodes-table
CREATE TABLE IF NOT EXISTS episodes (
    id SERIAL PRIMARY KEY,
    podcast_id INTEGER NOT NULL,
    title VARCHAR(500) NOT NULL,
    description VARCHAR(5000),
    published_at TIMESTAMP NOT NULL,
    duration_in_seconds INTEGER NOT NULL DEFAULT 0,
    audio_file_url VARCHAR(1000) NOT NULL,
    file_size_in_bytes BIGINT,
    progress_in_seconds INTEGER NOT NULL DEFAULT 0,
    is_downloaded BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_episodes_podcast FOREIGN KEY (podcast_id) REFERENCES podcasts(id) ON DELETE CASCADE
);

--changeset podcast:004-create-episodes-indexes
CREATE INDEX IF NOT EXISTS idx_episodes_podcast_id ON episodes(podcast_id);
CREATE INDEX IF NOT EXISTS idx_episodes_published_at ON episodes(published_at);

--changeset podcast:005-create-playlists-table
CREATE TABLE IF NOT EXISTS playlists (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description VARCHAR(1000),
    owner_id INTEGER NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_playlists_owner FOREIGN KEY (owner_id) REFERENCES users(id) ON DELETE CASCADE
);

--changeset podcast:005-create-playlists-index
CREATE INDEX IF NOT EXISTS idx_playlists_owner_id ON playlists(owner_id);

--changeset podcast:006-create-playlist-episodes-table
CREATE TABLE IF NOT EXISTS playlist_episodes (
    id SERIAL PRIMARY KEY,
    playlist_id INTEGER NOT NULL,
    episode_id INTEGER NOT NULL,
    "order" INTEGER NOT NULL DEFAULT 0,
    added_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_playlist_episodes_playlist FOREIGN KEY (playlist_id) REFERENCES playlists(id) ON DELETE CASCADE,
    CONSTRAINT fk_playlist_episodes_episode FOREIGN KEY (episode_id) REFERENCES episodes(id) ON DELETE CASCADE,
    CONSTRAINT uq_playlist_episodes UNIQUE (playlist_id, episode_id)
);

--changeset podcast:006-create-playlist-episodes-index
CREATE UNIQUE INDEX IF NOT EXISTS idx_playlist_episodes_unique ON playlist_episodes(playlist_id, episode_id);

