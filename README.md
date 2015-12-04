#Internet Museum of DramaTech Theatre

[IMDT](http://imdt.friendsofdt.org/) is an archive and history site operated by Friends of DramaTech.
Here you will find information on DramaTech going back to its beginnings in 1947.
We have information on 2300 people who have been involved with 388 shows.

For information on what's currently going on at DT, check out [DramaTech's official site](http://dramatech.org/).

#Technical Details

This site is [open source](https://github.com/jdaigle/friendsofdt), maintained by [Joseph Daigle](https://github.com/jdaigle) and hosted on GitHub at: [https://github.com/jdaigle/friendsofdt](https://github.com/jdaigle/friendsofdt).

The server-side component of the site is primarily ASP.NET MVC 5, and the site itself is hosted on Windows Azure.

#Roadmap

* [x] Build and deploy site artifacts and database schema to Azure. Deployment pipeline is complete.
* [x] Complete data migration. On demand data synchronization from existing site.
* [ ] Complete read-only views
  * [ ] Welcome Page
  * [x] Search/Search results
  * [x] Person Details
  * [x] Show Details
  * [ ] Awards by Year (e.g. Banquet)
  * [x] Photos for Person
  * [x] Photos for Show
  * [ ] Recent/Random Photos
  * [ ] List Shows (sort by year/title)
  * [ ] Club Positions (e.g. EC) By Year
  * [ ] Toaster List
  * [ ] Latest Updates
* [ ] Build authentication module. Custom OWIN middleware for Facebook login (ideally lighterweight than MS').
* [ ] Basic user admin tasks (manage users, roles, etc.).
* [ ] Complete admin tasks.