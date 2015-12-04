<?
include_once "include/get_fullname.php";

function awards_by_year($year) {
    $min_year_query = mysql_query ("select min(year),max(year) from awards");
    while ($min_year_result = mysql_fetch_array($min_year_query)) {
	$min_year = $min_year_result[0];
	$max_year = $min_year_result[1];
    }
    if (!isset($year)) {$year=$max_year;}
    if ($year > $max_year) {$year=$max_year;}
    if ($year < $min_year) {$year=$min_year;}
    $order_select = mysql_query("SELECT distinct year FROM awards order by year");
    while ($order_result = mysql_fetch_array($order_select)) {
        if (isset($next)) {
            if ($next == 'next') {
                $next = $order_result[0];
            }
        }
        if ($order_result[0] == $year && !isset($next)) {
            $next = 'next';
        } elseif (!isset($next)) {
            $last = $order_result[0];
        }
    }
    if ($last > 0) {
        $prevlink = "<a href=\"$_SERVER[PHP_SELF]?action=awards_by_year&amp;year=$last\"><--- Previous Year</a>";
    } else {
        $prevlink = "<--- Previous Year";
    }
    if ($next == 'next') {
        $nextlink = "Next Year --->";
    } else {
        $nextlink = "<a href=\"$_SERVER[PHP_SELF]?action=awards_by_year&amp;year=$next\">Next Year ---></a>\n";
    }

    echo "<table width=80% align=center><tr><td width=25%>$prevlink</td><td><center><h1>DT Banquet - Awards for $year</h1></center></td><td width=25% align=right>$nextlink</td></tr></table><center>";
    $award_select = mysql_query("select l.name,a.showID,a.peepID from awards a INNER JOIN awards_list l ON a.awardID=l.ID where a.year = $year order by a.awardID");
    while ($result = mysql_fetch_array($award_select)) {
        echo $result[0];
        if ($result[1]) {
            $show_select = mysql_query("select title from shows where ID = " . $result[1]);
            if ($show = mysql_fetch_array($show_select))
                echo " - <a href=\"?action=show_detail&amp;show_id=$result[1]\">$show[0]</a>";
            else
                echo " - Show not found";
        }
        if ($result[2])
            echo " - <a href=\"?action=peep_detail&amp;peep_id=$result[2]\">" . get_fullname($result[2]) . "</a>";
        echo "<br>\n";
    }
    echo "</center>";
}
?>
