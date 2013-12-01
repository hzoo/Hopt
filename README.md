Hopt
====

Hopt is a design tool to optimization ED room requirements.

### Setup ###
To run Hopt, first run the server, and then open the website.
1. To open the server: open `bin\Debug\HoptServer.exe`.
2. To open the website: open `bin\index.html` in Chrome or your up to date browser.

*note: you need Simio on your computer for the server to run properly.*

### Navigation ###
The Hopt tool is a website with 4 pages: Home, Inputs, Config, and Opt.  
These pages can be navigated using the toolbar on the top of the website.

1. Home: Explains what the tool does.
![](http://i.imgur.com/Ai6BH8C.png)
2. Inputs: Specify inputs for a specific hospital.
![](http://i.imgur.com/yJBI3LD.png)
3. Config: Test alternative configurations.
![](http://i.imgur.com/EaEcMlh.png)
4. Opt: Find the lowest cost room configuration.
![](http://i.imgur.com/TcJpGs6.png)

### Use ###
#### Change Inputs ####
The first thing you can do is change the inputs on the "Setup Inputs" page.  
You can change inputs related to the simulation, costs, or constraints.

Use the sidebar on the left to quickly move to a section. In the picture below, `Constraints` was clicked on, so the page scrolled up till the `Constraints` heading hits the top of the page.
![](http://i.imgur.com/8FSZVZw.png)  
For example: if you wanted to change the Hospital Name to `Emory` and Hospital Location to `USA`..
![](http://i.imgur.com/SnFuRaf.png)  
There are defaults for all the input fields in case there isn't data for the hospital you are working with.  

*Note: the arrivals should be the annual arrivals in 10 years since we are calculating the total cost over a 10 year period.*

*We weren't able to implement storing all the input, config, and optimization data so you will have to retype the information again after you start the website again.It is advised you store this information in a seperate document.*
#### Run Optimization ####
Next once all the inputs are filled in, you can go ahead and run to find the optimum room (minimum cost) configuration usings the inputs from the other page. Click on the last tab called `Search`.  ![](http://i.imgur.com/9jkE0At.png)

Here you just have to specify the existing configuration of the hospital in the first column of input boxes.
![](http://i.imgur.com/r5on1BH.png)  
The second column of input boxes is the maximum number of rooms for that room type based on the calculati
ons from the previously used excel model that overestimated rooms. This serves as a good boundary for our 
optimization. Here you are free to change the max boundaries if needed.  
![](http://i.imgur.com/1HzRUhh.png)  
Now just click `Run Opt`.  ![](http://i.imgur.com/sDTGh8f.png)  
After a few minutes, the results should come in with the recommended room configuration, 
![](http://i.imgur.com/FKid93S.png)  
performance measures,  
![](http://i.imgur.com/H0pNtMp.png)  
and the cost of the room configuration. The final cost is calculated assuming 5 years of construction and a 10 year forecast.  
![](http://i.imgur.com/OzVi7To.png)

#### Run Alternatives ####
Now maybe you want to try a different room configuration. Maybe the hospital has a suggestion. You can run a specific configuration and see the results. Click on the tab called `Run a Configuration`.  ![]()

This page has a few special/advanced options you probably won't need to change.
You can change the amount of time the simulation is run, the number of times it is run, and the startup time of the simulation.
![](http://i.imgur.com/BLZD71E.png)  
You can also run the simulation over an average day (annual visits / 365)  
or a peak month and average day (where the peak is x% of the year)  
or a peak month and a peak day (avg day + 2.33 * Math.sqrt(avg day) )  
![](http://i.imgur.com/f3hnrnK.png)  

So pretending you want to try using 100 of each room type, the input boxes under "New" would look like this:
![](http://i.imgur.com/B4w9yTJ.png)  
After you click "Run", wait a moment and some similar results will show up.  
The costs are broken down a bit more.  

The responses have a 2nd column. What this does is give you the difference in responses between the first configuration you run and the second configuration. So given that we ran the old configuration and the new one where each room type has 100 rooms, the first column has low responses for everything because there are too many rooms. The 2nd column shows you the difference between the old and the new. In this case all the room utilizations went down because we added a lot of rooms. Red text means the value went down, and green text means the value went up.
![](http://i.imgur.com/AP3IumU.png)  
At the bottom, there is also a table that records all the last configurations you have run so far. It isn't the most useful since it is rather cluttered.  
![](http://i.imgur.com/D6oHXhT.png)
