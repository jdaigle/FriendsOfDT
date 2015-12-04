<?

function stats() {
    include_once ("include/get_fullname.php");
    include_once ("include/get_title.php");
    echo "<center><font size=+3>IMDT Stats</font><br><br>\n";
    echo "<a href=\"?action=shows_by_year\">Browse Shows by Year</a><br><br>\n";
    echo "<a href=\"?action=shows_by_title\">Browse Shows by Title</a><br><br>\n";
    echo "<a href=\"?action=awards_by_year\">Browse Awards by Year</a><br><br>\n";
    echo "<a href=\"?action=EC_by_year\">Browse Positions by Year</a><br><br>\n";
    echo "<a href=\"?action=toaster_hunt\">Where was the Toaster?</a><br><br>\n";
    echo "<table width=80% align=center><tr><td align=center>\n";
    top_ten('cast','peepID','Top Ten Cast');
    echo "</td><td align=center>\n";
    top_ten('crew','peepID','Top Ten Crew');
    echo "</td></tr>\n<tr><td></td><td>&nbsp;</td></tr>\n<tr><td align=center>";
    top_ten('awards','peepID','Top Ten Awards (People)');
    echo "</td><td align=center>\n";
    top_ten('awards','showID','Top Ten Awards (Shows)');
    echo "</td></tr></table></center><br>\n";
}

function top_ten($type,$idtype,$title) {
    if ($type == 'awards') {
	$tocount=$idtype;
	$clarify = "where $idtype is not null";
    } else {
	$tocount='distinct showID';
	$clarify = "";
    }
    if ($idtype == 'peepID') {
	$kind='peep';
    } elseif ($idtype == 'showID') {
	$kind='show';
    }
    $top_ten_q = mysql_query("select $idtype,count($tocount) as numb from $type $clarify group by $idtype order by numb DESC limit 10");
    $type = ucfirst($type);
    echo "<table border=1 align=center><tr><td colspan=2 align=center><font size=+2>$title</font></td></tr>";
    echo "<tr><th align=center>Name</th><th>&nbsp;</th></tr>";
    while ($top_ten = mysql_fetch_array($top_ten_q)) {
	if ($idtype == 'showID') { 
	    $name=get_title($top_ten[0]); 
	} else {
	    $name=get_fullname($top_ten[0]);
	}
	echo "<tr><td align=center><a href=\"?action=" . $kind . "_detail&amp;" . $kind . "_id=$top_ten[0]\">$name</a></td><td>$top_ten[1]</td></tr>";
    }
    echo "</table>";
}
?>
