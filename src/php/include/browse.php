<?
include_once ("include/get_q.php");
include_once ("include/rearrange_title.php");

function browse_shows($sc) {
    $sort_criteria =  mysql_real_escape_string($sc);
    echo "<center><h1>All shows sorted by $sort_criteria</h1>";
    $show_select = mysql_query("SELECT ID,title,year,quarter FROM shows order by $sort_criteria,year,quarter,title");
    while ($shows = mysql_fetch_array($show_select)) {
        $show_id=$shows[0];
        $title=stripslashes($shows[1]);
	$title = rearrange_title($title);
        $quarter = get_q($shows[3]);
        echo "  <a href=\"$_SERVER[PHP_SELF]?action=show_detail&amp;show_id=$show_id\">$title ($quarter $shows[2])</a><br>\n";
    }
    echo "</center>";
}
?>
