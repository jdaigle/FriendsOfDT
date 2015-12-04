<?

include_once ("include/get_fullname.php");

function ECbrowse() {
    echo "<center><h1>EC sorted by year</h1><br><table align=center>";
	echo "<tr><th align=center>Year</th><th align=center>Title</th><th align=center>Name</th></tr>";
    $EC_select = mysql_query("select e.peepID,e.year,l.title from EC e INNER JOIN EC_list l ON e.ECID=l.ID order by e.year DESC,e.ECID,e.peepID");
    $oldyear=0;
    while ($ECresult = mysql_fetch_array($EC_select)) {
        $name=get_fullname($ECresult[0]);
        $year=$ECresult[1];
	if ($year == $oldyear) {
	    $year='&nbsp;';
	} else {
	    echo "<tr><td colspan=3>&nbsp;</td></tr>";
	    $oldyear = $year;
	}
	$title = $ECresult[2];
	echo "<tr><td align=center><b>$year</b></td><td align=right>$title &nbsp;- &nbsp;</td><td align=left><a href=\"$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=$ECresult[0]\">$name</td></tr>";
    }
    echo "</table>";
}
?>
