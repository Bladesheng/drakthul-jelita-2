-- Run this against the NEW database.
-- ```bash
-- sqlite3 database.sqlite < migrate-screenshots.sql
-- ```

ATTACH
DATABASE 'old.sqlite' AS old;

INSERT INTO main.Screenshots
(Id, Path, MimeType, Size, WowName, CreatedAt, WowClassId, Width, Height)
SELECT s.id,
       s.path,
       s.mime_type,
       s.size,
       s.wow_name,
       s.created_at,
       s.wow_class_id,
       s.width,
       s.height
FROM old.screenshots AS s;

-- These should match.
SELECT COUNT(*)
FROM old.screenshots;

SELECT COUNT(*)
FROM main.Screenshots;

DETACH
DATABASE old;
