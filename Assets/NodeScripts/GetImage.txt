var webshot = require('webshot');
var flatiron = require('flatiron');

var app = flatiron.app;

app.use(flatiron.plugins.http);

app.router.get('/getImage', function() {
	  var self = this;
	  var requestUrl = this.req.headers['head'];
	  console.log(requestUrl); 
	  webshot(requestUrl, function(err, renderStream) {
	    renderStream.pipe(self.res);
	  });
});

app.start(3000,"198.252.105.8");

console.log('Starting Node Server');




