// import express (after npm install express)
const express = require('express');
const bodyParser = require('body-parser');
// create new express app and save it as "app"
const app = express();

// server configuration
const PORT = 8055;

app.use(bodyParser.text());

app.get('/', (req, res) => {
  res.send('Hello World');
  console.log("Message");
});

app.post('/', (req, res) => {
  res.send('Hello World');
  console.log("Messagep");
});


// create a route for the app
app.post('/api/messages/:topic/:key', (req, res) => {
  res.send('Hello World');
  console.log("jgbody2");
  console.log(req.params.topic);
  console.log(req.params.key);
  console.log("--");
  console.log(req.body);
});

// make the server listen to requests
app.listen(PORT, () => {
  console.log(`Server running at: http://localhost:${PORT}/`);
});