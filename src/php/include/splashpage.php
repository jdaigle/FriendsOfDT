<?
function splashpage() {
    $num_show_query = mysql_query ("select count(distinct ID) from shows");
    while ($num_shows = mysql_fetch_array($num_show_query)) {
	$shows=$num_shows[0];
    }
    $data_show_query = mysql_query ("select count(distinct showID) from cast");
    while ($data_shows = mysql_fetch_array($data_show_query)) {
	$datashows=$data_shows[0];
    }
    $showsleft = ($shows - $datashows);
    $num_peep_query = mysql_query ("select count(distinct ID) from people");
    while ($num_peeps = mysql_fetch_array($num_peep_query)) {
	$peeps=$num_peeps[0];
    }
    $num_del_query = mysql_query ("select count(*) from people where fname='Deleted'");
    while ($num_del_result = mysql_fetch_array($num_del_query)) {
	$num_del=$num_del_result[0];
    }
    echo <<< EOT

    <div style="background-color: #d9edf7; text-align: center; color: #31708f; padding: 15px; font-size: 120%;">
        Heads up. <strong>We're building a new IMDT!</strong> You can check out the progress at
        <a href="http://friendsofdt.org">http://friendsofdt.org</a>. The new site may be out-of-date occasionally until
        it's finished.
        <br />
        <br />
        The new site is <a href="https://github.com/jdaigle/friendsofdt">open source</a> and feedback
        and contributions are not only welcome, they're encouraged! Details will be available on the new site's
        welcome page.
    </div>

	<br><br><center><p><font size=+2>Welcome to IMDT - the <b>I</b>nternet <b>M</b>useum of <b>D</b>ramaTech <b>T</b>heatre!<br><br></font>

	<b>Check out the new <a href="?action=media_index">Media Page</a>!  You can now <a href="?action=media_upload">Upload pictures</a> 
	and tie them to people and shows.  Log in and check it out!
	<br><br>
	Also new is the <a href="?action=toaster_hunt">Toaster Hunt Page</a>.  See where the toaster was hidden in each show!</b>
	<br><br>

	DramaTech Theatre is Atlanta's oldest continually operating theatre.<br>
	Here you will find information on DramaTech going back to its beginnings in 1947.<br>
	We have information on $peeps people who have been involved with $shows shows.<br>
	For more fun facts, check out our <a href="?action=stats">Stats Page</a>.<br><br>

	Members of the site are allowed to edit their personal information,upload a picture of themselves, and even
	print out a personalized resum&eacute; of their participation at DT.<br>
	If you're not already registered, you can do so with the links at the top of the page.<br><br>

	Our work is far from over - there are still at least $showsleft shows that need cast and crew data entered.<br>
	If you are interested in volunteering to do data entry or you have information on some of these
	"orphaned" shows, please <a href="mailto:fodtadmin@cridion.com">Contact the DataMaster.</a><br><br>

	For information on what's currently going on at DT, check out <a target='_blank' href="http://dramatech.org">
	Dramatech's official site</a>.<br><br>

	This project could not have been possible without
	<a target='_blank' href="http://friendsofdt.org">
	Friends of DramaTech</a>.  If you're an alum, check out their site too.<br><br>

	Special and specific thanks to
	<a href="$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=857">all</a> the
	<a href="$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=377">people</a> who
	<a href="$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=92">helped</a>
	<a href="$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=77">with</a>
	<a href="$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=1112">data</a>
	<a href="$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=230">entry</a>.<br><br>
	If you care to see it, here's the <a href="changelog.txt" target="_blank">Site's Change Log</a>.
	</center>
EOT;
}
#	Due to bone-headedness on the part of the administrator (me), the 'people' table was erased this morning (September 13).<br>
#	Any people added between 7/12 and 9/12 will be listed as 'Deleted Person' until fixed (<b>currently $num_del</b>).<br>
#	Those people whose personal records were edited during that time have been reverted back to their July 12 state.<br>
#	If you signed up for the DB in the affected span, please re-register.  Sorry!<br><br>
?>
