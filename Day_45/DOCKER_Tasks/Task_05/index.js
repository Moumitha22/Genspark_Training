const express = require('express');
const { MongoClient } = require('mongodb');
const app = express();
const PORT = 3000;

app.get('/', async (req, res) => {
  try {
    const client = await MongoClient.connect(process.env.MONGO_URL);
    const db = client.db();
    const collections = await db.collections();

    res.json({
      message: 'Connected to MongoDB!',
      collections: collections.map(c => c.collectionName)
    });

    client.close();
  } catch (err) {
    res.status(500).json({ error: 'MongoDB connection failed', details: err.message });
  }
});

app.listen(PORT, () => {
  console.log(`Node API running at http://localhost:${PORT}`);
});
