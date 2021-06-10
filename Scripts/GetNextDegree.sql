SELECT 
    n1.nconst,
    n1.primaryName,
    p1.category,
    p1.characters,
    --t1.tconst,
    t1.primaryTitle,
    t1.titleType,
    n2.nconst,
    n2.primaryName,
    p2.category,
    p2.characters
FROM name n1
JOIN principal p1
    ON n1.nconst = p1.nconst
JOIN title t1
    ON p1.tconst = t1.tconst
JOIN principal p2
    ON t1.tconst = p2.tconst
JOIN name n2
    ON p2.nconst = n2.nconst
WHERE n1.nconst = 'nm1024677'
    AND n1.nconst <> n2.nconst;