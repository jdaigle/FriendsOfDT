<?

function get_search_list() {
    $searchfield=$_REQUEST['searchfield'];
    if (strlen($searchfield) <1) {include_once ("splashpage.php"); splashpage(); endlines();}
    $searchstrings = explode(" ", $searchfield);
    for ($i = 0; $i < sizeof($searchstrings); $i++) {
        if ($searchstrings[$i] == "") continue;

        if ($i) {
            $peep_query .= " and ";
            $show_query .= " and ";
        }
	$s = mysql_real_escape_string($searchstrings[$i]);
        $peep_query .= "(lname like '%" . $s . "%' or fname like '%" . $s . "%' or nickname like '%" . $s . "%')";
        $show_query .= "(title like '%" . $s . "%')";
    }

    if ($_REQUEST['searchtype'] == 'show')
        $query = mysql_query("(select ID, concat(title, ' (', year, ')') as title, 'show' as type from shows where $show_query) order by title");
    else if ($_REQUEST['searchtype'] == 'peep')
        $query = mysql_query("(select ID, concat(lname, ', ', fname) as title, 'peep' as type from people where $peep_query) order by title");
    else
        $query = mysql_query("(select ID, concat(lname, ', ', fname) as title, 'peep' as type from people where $peep_query) union all (select ID, concat(title, ' (', year, ')') as title, 'show' as type from shows where $show_query) order by title");

    if (mysql_num_rows($query) == 1) {
	while ($result = mysql_fetch_array($query)) {
	    $id = $result['ID'];
	    $type = $result['type'];
	    $action = "show_" . $type . "_detail";
	    $to_include = "include/" . $type . "_detail.php";
	    $idtype = $type . "_id";
	}
	include_once ("$to_include");
	$action($id);
	endlines();
    }
    echo "<h1>Possible Matches for '" . $searchfield . "':</h1><br>\n";
    if (mysql_num_rows($query) < 1) {echo "<h1>No Records found!</hi>\n"; endlines();}
    include_once ("include/get_fullname.php");
    include_once ("include/get_pic.php");
    $counter = 0;
    echo "<table align=center border=1><tr>";
    while ($result = mysql_fetch_array($query)) {
	$counter++;
	if ($counter == 5) {
	    echo "</tr><tr>";
	    $counter = 1;
	}
	$id = $result['ID'];
	$type = $result['type'];
	$action = $type . "_detail";
	$idtype = $type . "_id";
	$picture = get_tinypic($type,$id);
	if ($type == 'peep')
	    $fullname = get_fullname($id);
	else
	    $fullname = $result['title'];
	echo <<< EOT7
	<td align=center valign=middle>
	<div class="rollover">
	<a href="?action=$action&amp;$idtype=$id">
	<img src="$picture"><br>$fullname</a></div></td>
EOT7;
    }
    echo "</tr></table>";
}

?>
