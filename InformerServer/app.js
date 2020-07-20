var express = require('express')
var config = require('./config.json')
var fs = require("fs")
var app = express();

var addr;
var lastUpdate;

if(config.authorize){
    if(config.key == undefined){
        config.key = Math.random().toString(36).slice(2);
        console.log("You didn't specify a key, a key generated.\n"+config.key)

        fs.writeFileSync('./config.json', JSON.stringify(config))
    }
    app.use('/addr',function(req,res,next){
        if(req.headers['authorization'] == config.key)
            next();
        else{
            res.status(401)
            next("Unauthorized")
        }
    })
}
app.use(express.json())

app.get('/addr', function(req,res){
    if(lastUpdate == undefined || Date.now() - lastUpdate > config.expires * 1000)
        res.send("out of date")
    else
        res.send(addr)
})

app.post('/addr', function(req, res){
    addr = req.body.addr;
    lastUpdate = Date.now();
    res.send(res.json(req.body))
})

app.get('/', function (req, res) {
    res.send('Hello World!');
  });

app.listen(3000, function () {
  console.log('Example app listening on port 3000!');
});