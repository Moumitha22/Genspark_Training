const express = require('express');
const cors = require('cors'); 

const app = express();
const PORT = 3000;

app.use(cors()); 

app.get('/api', (req, res) => {
  res.json({ message: 'Hello from backend!' });
});

app.listen(PORT, () => {
  console.log(`Backend API running at http://localhost:${PORT}`);
});
