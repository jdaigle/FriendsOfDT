<?

function dbcleanup_blanks() {
    # Function checks for blank entries in the DB.  These are backed up and deleted.
    include_once ("include/add_del.php");
    $blank = array(
	'EC' => "select ID from EC where peepID is null and ECID is null",
	'EC_list' => "select ID from EC_list where title is null",
	'awards' => "select ID from awards where awardID is null",
	'awards_list' => "select ID from awards_list where name is null",
	'cast' => "select ID from cast where peepID is null and role is null and showID is null",
	'crew' => "select ID from crew where peepID is null or jobID is null",
	'jobs' => "select ID from jobs where job is null",
	'people' => "select ID from people where lname is null and fname is null",
	'shows' => "select ID from shows where title is null"
    );
    foreach ($blank as $table => $query) {
	$counter = 0;
	$select = mysql_query($query);
	$rowcheck = mysql_num_rows($select);
	if ($rowcheck > 0) {
	    while ($ID_to_del = mysql_fetch_array($select)) {
		delitem($table,$ID_to_del[0]);
		$counter ++;
	    }
	}
	echo "Deleted $counter rows from $table<br>\n";
    }
}

?>
