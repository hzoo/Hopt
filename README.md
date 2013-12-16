#[Hopt](http://eltacodeldiablo.github.io/gtsd13)#
======

Emergency Department Design tool created for GT ISYE Senior Design.

## Steps to Build/Run ##

### Server
- *Server will not run without [Simio](http://www.simio.com) installed.*
- To just run: Open HoptServer\bin\Debug\HoptServer.exe 
- To develop: Install Visual Studio, install .NET Framework 4.5

### Client (Runs in browser)
Install [Node.js](http://nodejs.org/) and then:

```sh
$ git clone git://github.com/eltacodeldiablo/gtsd13
$ cd hopt
$ sudo npm -g install grunt-cli karma bower
$ npm install
$ bower install
$ grunt watch
```

to run: open HoptClient\build\index.html or HoptClient\bin\index.html after running grunt.

## Technologies Used ##

### Front End ###
- [Angular.js]
	- [Angular UI]
		- UI Router
		- UI Bootstrap
		- ng-grid 
- [ng-boilerplate]
	- [Bootstrap]
	- [jQuery]

### Back End ###
- Simio DLLs
- [ASP.NET]
	- [SignalR]

### Tools ###
* Build
	* [Grunt]
* IDE
	* [Sublime Text]
	* [Visual Studio 2013]
* Browser
	* [Chrome]
* Version Control
	* [Github]
* Server
	* Github pages
	* [ngrok]
	* [node]
		* [npm]
		* [http-server]
 

License
--------
MIT

[Visual Studio 2013]: http://microsoft.com/visualstudio
[Sublime Text]: http://sublimetext.com
[ng-boilerplate]: http://joshdmiller.github.io/ng-boilerplate/
[Chrome]: http://google.com/chrome
[Github]: http://github.com
[Bootstrap]: http://getbootstrap.com
[Angular.js]: http://angularjs.org/
[jQuery]: http://www.jquery.com
[Angular UI]: http://angular-ui.github.io/
[ASP.NET]: http://www.asp.net/
[SignalR]: http://www.asp.net/signalr
[ngrok]: https://ngrok.com/
[node]: http://nodejs.org/
[npm]: https://npmjs.org/
[http-server]: https://npmjs.org/package/http-server
[Grunt]: http://gruntjs.com/
[Karma]: http://karma-runner.github.io/
