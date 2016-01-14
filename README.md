#Friends of DramaTech Official Website

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
  * [x] Awards by Year (e.g. Banquet)
  * [x] Photos for Person
  * [x] Photos for Show
  * [ ] <del>Recent/Random Photos</del>
  * [ ] List Shows (sort by year/title)
  * [ ] Club Positions (e.g. EC) By Year
  * [ ] Toaster List
  * [x] Latest Updates
* [ ] Build authentication module. Facebook login (ideally lighterweight than MS').
  * [ ] basic user profile (name, email, contact pref.)
* [ ] Basic user admin tasks (manage users, roles, etc.).
  * [ ] "Contributor" = Can edit own personal info, upload photos, suggest edits
  * [x] "Archivist" = Can add/edit shows/people/etc. approve suggested edits. delete.
  * [x] "Admin" = Can approve new users, change roles.
* [ ] Complete feature-parity admin tasks.
  * [x] Tagging Photos
  * [x] Deleting Photo Tags
  * [x] Deleting Photos
  * [x] Uploading Photos
  * [ ] Person: add/remove awards
  * [ ] Person: add/remove EC
  * [ ] Person: add/remove cast
  * [ ] Person: add/remove crew
  * [ ] Person: edit name
  * [ ] Person: edit bio
  * [ ] Person: Primary Photo
  * [ ] Person: Add
  * [ ] Person: Remove
  * [ ] Show: add/remove awards
  * [ ] Show: add/remove cast
  * [ ] Show: add/remove crew
  * [ ] Show: edit name
  * [ ] Show: edit author
  * [ ] Show: edit year/quarter
  * [ ] Show: edit desc/toaster/etc
  * [ ] Show: Primary Photo
  * [ ] Show: Add
  * [ ] Show: Remove
  * [ ] Award Types: add/remove/rename
* [ ] Edit Suggestion. In lieu of being able to edit, a contributor can suggest edits to a page. The archivist can review and implement the edits.
* [ ] Show Types - Additional metadata
  * [ ] "Banquet" should be linked to award year. One Banquet per calendar year.
  * [ ] "Open House"?
  * [ ] Promote certain crew positions (directory, designers, crew chiefs) for searching
  * [ ] crew position cleanup (spelling, dups, etc)