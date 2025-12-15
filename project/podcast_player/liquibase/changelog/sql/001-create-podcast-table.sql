
create table if not exists podcasts (
    id serial primary key,
    name varchar(255) not null,
    author varchar(255) not null,
    created_at timestamp not null default current_timestamp,
    updated_at timestamp not null default current_timestamp
);

create index if not exists idx_podcasts_name on podcasts(name);


